/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Core.Common;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Validation;
using StateBudgetEntity = SimulasiAPBN.Core.Models.StateBudget;

namespace SimulasiAPBN.Web.Pages.Dashboard.Policy
{  
    public class StateBudget : BasePage
    {
        private readonly IValidatorFactory _validatorFactory;

        public StateBudget(
            IConfiguration configuration,
            IMemoryCache memoryCache,
            IUnitOfWork unitOfWork, 
            IValidatorFactory validatorFactory) 
            : base(configuration, memoryCache, unitOfWork)
        {
            _validatorFactory = validatorFactory;
            AllowedRoles = new List<AccountRole>
            {
                AccountRole.Administrator
            };
        }

        public StateBudgetEntity ActiveStateBudget { get; private set; }
        public string ActiveStateBudgetName { get; private set; }
        public ICollection<StateBudgetEntity> AvailableStateBudgets { get; private set; }
        public decimal GrossDomesticProduct { get; private set; }
        public StateBudgetEntity LatestStateBudget { get; private set; }
        public ICollection<StateBudgetEntity> StateBudgets { get; private set; }


        public const string AddStateBudgetAction = "AddStateBudgetAction";
        public const string RemoveStateBudgetAction = "RemoveStateBudgetAction";

        private async Task<int> GetAvailableStateBudgetRevision(int year)
        {
            var stateBudgets = (await UnitOfWork.StateBudgets
                    .GetByYearAsync(year))
                .ToList();
            var currentRevision = !stateBudgets.Any() ? -1 :
                stateBudgets.Select(m => m.Revision)
                    .Max();
            return currentRevision + 1;
        }

        protected override async Task Initialize()
        {
            // Initialize base
            await base.Initialize();

            // Initialise variables with default value
            ActiveStateBudget = new StateBudgetEntity();
            ActiveStateBudgetName = "Anggaran Pendapatan dan Belanja Negara";
            AvailableStateBudgets = new Collection<StateBudgetEntity>();
            GrossDomesticProduct = 1m;
            LatestStateBudget = new StateBudgetEntity();
            StateBudgets = new Collection<StateBudgetEntity>();
            
            // Populate and throw based on facts
            var currentYear = DateTimeOffset.Now.Year;
            for (var year = currentYear; year >= currentYear - 2; year--)
            {
                AvailableStateBudgets.Add(new StateBudgetEntity
                {
                    Year = year,
                    Revision = await GetAvailableStateBudgetRevision(year)
                });
            }

            StateBudgets = (await UnitOfWork.StateBudgets.GetAllAsync())
                .ToList();
            
            ActiveStateBudget = UnitOfWork.StateBudgets.GetActiveFromList(StateBudgets);
            ActiveStateBudgetName = ActiveStateBudget is not null 
                ? Formatter.GetStateBudgetPolicyName(ActiveStateBudget)
                : "Tidak Ada";
            LatestStateBudget = UnitOfWork.StateBudgets.GetLatestFromList(StateBudgets);

            if (StateBudgets.Any() && ActiveStateBudget is null)
            {
                if (StateBudgets.Any(e => e.Year == currentYear))
                {
                    SetWarningAlert("Tidak ada Kebijakan Anggaran Pendapatan dan Belanja Negara (APBN) yang " +
                        $"berlaku untuk Tahun Anggaran {currentYear}. Untuk membuat APBN berlaku, silakan atur " +
                        "Pendapatan Negara dan Belanja Negara.");
                } 
                else
                {
                    SetErrorAlert("Tidak ada Kebijakan Anggaran Pendapatan dan Belanja Negara (APBN) untuk " +
                        $"Tahun Anggaran {currentYear}.");
                }
            }

            GrossDomesticProduct = await UnitOfWork.SimulationConfigs.GetGrossDomesticProductAsync();
        }
        
        public async Task OnGet()
        {
            try
            {
                await Initialize();
            }
            catch (Exception e)
            {
                SetErrorAlert(e.Message);
                if (!(e is GenericException)) throw;
            }
        }

        public async Task OnPost([FromForm] string action, StateBudgetEntity model)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();
                if (action is null) return;
                switch (action)
                {
                    case {} a when a == AddStateBudgetAction:
                        await AddStateBudget(model);
                        break;
                    case {} a when a == RemoveStateBudgetAction:
                        await RemoveStateBudget(model);
                        break;
                }
            }
            catch (Exception e)
            {
                await UnitOfWork.RollbackAsync();
                SetErrorAlert(e.Message);
                if (!(e is GenericException)) throw;
            }
        }

        private async Task AddStateBudget(StateBudgetEntity model)
        {
            (await _validatorFactory.StateBudget.GetValidationAsync(model))
                .ThrowIfInvalid();
            
            var stateBudget = new StateBudgetEntity
            {
                Year = model.Year,
                Revision = await GetAvailableStateBudgetRevision(model.Year),
            };
            await UnitOfWork.StateBudgets.AddAsync(stateBudget);
            
            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Kebijakan {Formatter.GetStateBudgetPolicyName(stateBudget)} " +
                            $"telah ditambahkan.");
        }

        private async Task RemoveStateBudget(GenericModel model)
        {
            var stateBudget = await UnitOfWork.StateBudgets.GetByIdAsync(model.Id);
            if (stateBudget is null)
            {
                await Initialize();
                SetErrorAlert("Gagal menghapus kebijakan APBN. " +
                              "Kebijakan APBN tersebut tidak ditemukan.");
                return;
            }
            
            var allowedRevision = await GetAvailableStateBudgetRevision(stateBudget.Year) - 1;
            if (stateBudget.Revision != allowedRevision)
            {
                await Initialize();
                SetErrorAlert($"Kebijakan {Formatter.GetStateBudgetPolicyName(stateBudget)} tidak " +
                              $"dapat dihapus karena terdapat APBN Perubahan yang lebih baru pada " +
                              $"tahun tersebut. Silakan hapus APBN Perubahan baru terlebih dahulu " +
                              $"sebelum menghapus kebijakan ini.");
                return;
            }

            var stateExpenditures = (await UnitOfWork.StateExpenditures
                    .GetByStateBudgetAsync(stateBudget))
                .ToList();
            foreach (var stateExpenditure in stateExpenditures)
            {
                var stateExpenditureAllocations = await UnitOfWork.StateExpenditureAllocations
                    .GetByStateExpenditureAsync(stateExpenditure);
                await UnitOfWork.StateExpenditureAllocations.RemoveRangeAsync(stateExpenditureAllocations);
            }
            await UnitOfWork.StateExpenditures.RemoveRangeAsync(stateExpenditures);

            var specialPolicies = (await UnitOfWork.SpecialPolicies.GetByStateBudgetAsync(stateBudget))
                .ToList();
            var specialPolicyAllocations = specialPolicies.SelectMany(e => e.SpecialPolicyAllocations);
            await UnitOfWork.SpecialPolicyAllocations.RemoveRangeAsync(specialPolicyAllocations);
            await UnitOfWork.SpecialPolicies.RemoveRangeAsync(specialPolicies);

            await UnitOfWork.StateBudgets.RemoveAsync(stateBudget);
            
            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Kebijakan {Formatter.GetStateBudgetPolicyName(stateBudget)} " +
                            $"telah dihapus.");
        }
    }
}
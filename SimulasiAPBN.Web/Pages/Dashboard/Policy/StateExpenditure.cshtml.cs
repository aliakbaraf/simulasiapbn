/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
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

namespace SimulasiAPBN.Web.Pages.Dashboard.Policy
{
    public class StateExpenditure : BasePage
    {
        private readonly IValidatorFactory _validatorFactory;

        public StateExpenditure(
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
        
        [BindProperty(SupportsGet = true)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global MemberCanBePrivate.Global
        public string StateBudgetId { get; set; }
        
        public ICollection<Budget> AvailableBudgets { get; private set; }
        public ICollection<Budget> Budgets { get; private set; }
        public Core.Models.StateBudget StateBudget { get; private set;  }
        public ICollection<Core.Models.StateBudget> StateBudgets { get; private set; }
        public ICollection<Core.Models.StateExpenditure> StateExpenditures { get; private set; }
        public decimal TotalAllocation { get; private set; }

        public const string AddStateExpenditureAction = "AddStateExpenditureAction";
        public const string ModifyStateExpenditureAction = "ModifyStateExpenditureAction";
        public const string RemoveStateExpenditureAction = "RemoveStateExpenditureAction";

        protected override async Task Initialize()
        {
            // Populate base
            await base.Initialize();
            
            // Initialise variables with default value
            AvailableBudgets = new List<Budget>();
            Budgets = new List<Budget>();
            StateBudget = new Core.Models.StateBudget();
            StateBudgets = new List<Core.Models.StateBudget>();
            StateExpenditures = new List<Core.Models.StateExpenditure>();
            TotalAllocation = 0;
            
            // Populate based on facts
            Budgets = (await UnitOfWork.Budgets.GetAllAsync())
                .OrderBy(budget => budget.Function)
                .ToList();
            
            StateBudgets = (await UnitOfWork.StateBudgets.GetAllAsync())
                .OrderBy(stateBudget => stateBudget.Year)
                .ThenBy(stateBudget => stateBudget.Revision)
                .ToList();
            if (!StateBudgets.Any())
            {
                return;
            }
            
            if (string.IsNullOrEmpty(StateBudgetId))
            {
                StateBudget = UnitOfWork.StateBudgets.GetLatestFromList(StateBudgets);
            }
            else
            {
                if (!Guid.TryParse(StateBudgetId, out var stateBudgetGuid))
                {
                    throw new BadRequestException("Kebijakan APBN tidak valid.");
                }
                StateBudget = await UnitOfWork.StateBudgets.GetByIdAsync(stateBudgetGuid);
            }

            if (StateBudget is null)
            {
                StateBudget = new Core.Models.StateBudget();
                throw new BadRequestException(
                    "Data Kebijakan APBN yang Anda pilih tidak ditemukan.");
            }

            StateExpenditures = (await UnitOfWork.StateExpenditures.GetByStateBudgetAsync(StateBudget))
                .ToList();
            TotalAllocation = StateExpenditures
                .Sum(expenditure => expenditure.TotalAllocation);

            foreach (var budget in Budgets)
            {
                if (StateExpenditures.All(stateExpenditure => stateExpenditure.BudgetId != budget.Id))
                {
                    AvailableBudgets.Add(budget);
                }
            }
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

        public async Task OnPost([FromForm] string action, Core.Models.StateExpenditure model)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();
                if (string.IsNullOrEmpty(action)) return;
                switch (action)
                {
                    case {} a when a == AddStateExpenditureAction:
                        await AddStateExpenditure(model);
                        break;
                    case {} a when a == ModifyStateExpenditureAction:
                        await ModifyStateExpenditure(model);
                        break;
                    case {} a when a == RemoveStateExpenditureAction:
                        await RemoveStateExpenditure(model);
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

        private async Task AddStateExpenditure(Core.Models.StateExpenditure model)
        {
            (await _validatorFactory.StateExpenditure.GetValidationAsync(model))
                .ThrowIfInvalid();

            var budget = await UnitOfWork.Budgets.GetByIdAsync(model.BudgetId);
            if (budget is null)
            {
                throw new BadRequestException("Data Anggaran yang Anda pilih tidak ditemukan.");
            }

            if (StateExpenditures.Any(expenditure => expenditure.Budget == budget))
            {
                throw new BadRequestException(
                    $"Anggaran {budget.Function} telah ada " +
                    $"{Formatter.GetStateBudgetPolicyName(StateBudget)}.");
            }
            
            var stateExpenditure = new Core.Models.StateExpenditure
            {
                Budget = budget,
                SimulationMaximumMultiplier = model.SimulationMaximumMultiplier / 100,
                StateBudget = StateBudget,
                TotalAllocation = model.TotalAllocation,
            };
            await UnitOfWork.StateExpenditures.AddAsync(stateExpenditure);
            
            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Anggaran {budget.Function} telah ditambahkan dalam " +
                            $"{Formatter.GetStateBudgetPolicyName(StateBudget)}.");
        }

        private async Task ModifyStateExpenditure(Core.Models.StateExpenditure model)
        {
            (await _validatorFactory.StateExpenditure.GetValidationAsync(model))
                .ThrowIfInvalid();

            var stateExpenditure = await UnitOfWork.StateExpenditures.GetByIdAsync(model.Id);
            if (stateExpenditure is null)
            {
                throw new BadRequestException("Data Belanja Negara tidak ditemukan.");
            }

            var budget = await UnitOfWork.Budgets.GetByIdAsync(model.BudgetId);
            if (budget is null)
            {
                throw new BadRequestException("Data Anggaran yang Anda pilih tidak ditemukan.");
            }

            if (stateExpenditure.Budget != budget &&
                StateExpenditures.Any(expenditure => expenditure.Budget == budget))
            {
                throw new BadRequestException(
                    $"Anggaran {budget.Function} telah ada " +
                    $"{Formatter.GetStateBudgetPolicyName(StateBudget)}.");
            }

            stateExpenditure.Budget = budget;
            stateExpenditure.SimulationMaximumMultiplier = model.SimulationMaximumMultiplier / 100;
            stateExpenditure.TotalAllocation = model.TotalAllocation;
            await UnitOfWork.StateExpenditures.ModifyAsync(stateExpenditure);

            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert(
                $"Anggaran {budget.Function} pada {Formatter.GetStateBudgetPolicyName(StateBudget)} " +
                $"telah diubah.");
        }

        private async Task RemoveStateExpenditure(GenericModel model)
        {
            var stateExpenditure = await UnitOfWork.StateExpenditures
                .GetByIdAsync(model.Id);
            if (stateExpenditure is null)
            {
                throw new BadRequestException("Data Belanja Negara tidak ditemukan.");
            }

            var stateExpenditureAllocations = await UnitOfWork.StateExpenditureAllocations
                .GetByStateExpenditureAsync(stateExpenditure);

            await UnitOfWork.StateExpenditureAllocations.RemoveRangeAsync(stateExpenditureAllocations);
            await UnitOfWork.StateExpenditures.RemoveAsync(stateExpenditure);

            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Anggaran {stateExpenditure.Budget.Function} pada " +
                            $"{Formatter.GetStateBudgetPolicyName(StateBudget)} telah dihapus.");
        }
    }
}
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
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Common.Exceptions;

namespace SimulasiAPBN.Web.Pages.Dashboard.Budgeting
{
    public class BudgetDetail : BasePage
    {

        public BudgetDetail(IConfiguration configuration, IMemoryCache memoryCache, IUnitOfWork unitOfWork) 
            : base(configuration, memoryCache, unitOfWork)
        {
            AllowedRoles = new List<AccountRole>
            {
                AccountRole.Administrator
            };
        }
        
        [BindProperty(SupportsGet = true)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global MemberCanBePrivate.Global
        public string BudgetId { get; set; }
        
        public string BudgetName { get; set; }
        public Core.Models.Budget Budget { get; private set; }
        public const string BudgetDescriptionIdentifier = "Description";

        protected override async Task Initialize()
        {
            // Initialize base
            await base.Initialize();

            // Initialise variables with default value
            BudgetName = "Anggaran";
            Budget = new Core.Models.Budget();
            
            // Populate based on facts
            if (string.IsNullOrEmpty(BudgetId))
            {
                throw new BadRequestException("Mohon pilih Anggaran.");
            }
            
            if (!Guid.TryParse(BudgetId, out var budgetGuid))
            {
                throw new BadRequestException("Data Anggaran tersebut tidak valid.");
            }
            
            Budget = await UnitOfWork.Budgets.GetByIdAsync(budgetGuid);
            if (Budget is null)
            {
                Budget = new Core.Models.Budget();
                throw new BadRequestException("Data Anggaran tersebut tidak ditemukan.");
            }

            BudgetName = $"Anggaran {Budget.Function}";
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

        public async Task OnPost()
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();

                var budgetTargets = new Collection<BudgetTarget>();
                foreach (var (key, values) in Request.Form)
                {
                    if (key == BudgetDescriptionIdentifier)
                    {
                        await SaveDescription(values.ToString());
                        continue;
                    }

                    if (!int.TryParse(key, out var index))
                    {
                        continue;
                    }
                    
                    var budgetTargetDescription = values.ToString();
                    if (string.IsNullOrEmpty(budgetTargetDescription))
                    {
                        throw new BadRequestException($"Sasaran nomor { index + 1 } kosong. " +
                                                      $"Mohon isi seluruh sasaran.");
                    }
                    
                    budgetTargets.Add(new BudgetTarget
                    {
                        BudgetId = Budget.Id,
                        Description = budgetTargetDescription
                    });
                }

                await SaveBudgetTargets(budgetTargets);

                await Initialize();
                await UnitOfWork.CommitAsync();
                
                SetSuccessAlert($"Detil {BudgetName} telah disimpan.");
            }
            catch (Exception e)
            {
                await UnitOfWork.RollbackAsync();
                SetErrorAlert(e.Message);
                if (!(e is GenericException)) throw;
            }
        }

        private async Task SaveDescription(string description)
        {
            Budget.Description = description;
            await UnitOfWork.Budgets.ModifyAsync(Budget);
        }

        private async Task SaveBudgetTargets(IEnumerable<BudgetTarget> budgetTargets)
        {
            var currentIgnoreSoftDeleteProperty = UnitOfWork.BudgetTargets.Options.IgnoreSoftDeleteProperty;
            UnitOfWork.BudgetTargets.Options.IgnoreSoftDeleteProperty = true;
            await UnitOfWork.BudgetTargets.RemoveRangeAsync(Budget.BudgetTargets);
            UnitOfWork.BudgetTargets.Options.IgnoreSoftDeleteProperty = currentIgnoreSoftDeleteProperty;
            
            if (budgetTargets.Any())
            {
                await UnitOfWork.BudgetTargets.AddRangeAsync(budgetTargets);
            }
        }
        
    }
}
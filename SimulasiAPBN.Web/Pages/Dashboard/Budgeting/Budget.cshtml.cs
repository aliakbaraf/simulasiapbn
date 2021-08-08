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
using SimulasiAPBN.Web.Validation;
using BudgetEntity = SimulasiAPBN.Core.Models.Budget;

namespace SimulasiAPBN.Web.Pages.Dashboard.Budgeting
{
    public class Budget : BasePage
    {
        private readonly IValidatorFactory _validatorFactory;

        public Budget(
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
        
        public ICollection<BudgetEntity> Budgets { get; set; }

        public const string AddBudgetAction = "AddBudgetAction";
        public const string RemoveBudgetAction = "RemoveBudgetAction";

        protected override async Task Initialize()
        {
            // Initialize base
            await base.Initialize();
            
            // Initialise variables with default value
            Budgets = new Collection<BudgetEntity>();
            
            // Populate based on facts
            var budgets = await UnitOfWork.Budgets.GetAllAsync();
            Budgets = budgets.OrderBy(budget => budget.Function)
                .ToList();
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

        public async Task OnPost([FromForm] string action, BudgetEntity model)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();
                if (string.IsNullOrEmpty(action)) return;
                switch (action)
                {
                    case AddBudgetAction:
                        await AddBudget(model);
                        break;
                    case RemoveBudgetAction:
                        await RemoveBudget(model);
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

        private async Task AddBudget(BudgetEntity model)
        {
            (await _validatorFactory.Budget.GetValidationAsync(model))
                .ThrowIfInvalid();

            var budget = new BudgetEntity
            {
                Function = model.Function,
                Description = "",
                Type = model.Type,
            };
            await UnitOfWork.Budgets.AddAsync(budget);

            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Anggaran {budget.Function} telah ditambahkan.");
        }

        private async Task RemoveBudget(GenericModel model)
        {
            var budget = await UnitOfWork.Budgets.GetByIdAsync(model.Id);
            if (budget is null)
            {
                throw new BadRequestException("Anggaran tersebut tidak ditemukan.");
            }

            var stateExpenditures = (await UnitOfWork.StateExpenditures
                    .FindAsync(e => e.BudgetId == budget.Id))
                .ToList();
            if (stateExpenditures.Any())
            {
                throw new BadRequestException(
                    $"Tidak dapat menghapus Anggaran {budget.Function}. Anggaran {budget.Function} masih " +
                    $"terkait dengan salah satu APBN. Silakan hapus Anggaran {budget.Function} dari seluruh APBN " +
                    "terlebih dahulu.");
            }

            await UnitOfWork.BudgetTargets.RemoveRangeAsync(budget.BudgetTargets);
            await UnitOfWork.Budgets.RemoveAsync(budget);
            
            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Anggaran {budget.Function} telah dihapus.");
        }
    }
}
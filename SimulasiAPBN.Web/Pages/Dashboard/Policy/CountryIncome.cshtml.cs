/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Core.Common;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Web.Common.Exceptions;
using StateBudgetEntity = SimulasiAPBN.Core.Models.StateBudget;

namespace SimulasiAPBN.Web.Pages.Dashboard.Policy
{
    public class CountryIncome : BasePage
    {
        public CountryIncome(IConfiguration configuration, IMemoryCache memoryCache, IUnitOfWork unitOfWork) 
            : base(configuration, memoryCache, unitOfWork)
        {
            AllowedRoles = new List<AccountRole>
            {
                AccountRole.Administrator
            };
        }
        
        public StateBudgetEntity ActiveStateBudget { get; set; }
        public IEnumerable<StateBudgetEntity> StateBudgets { get; set; }

        protected override async Task Initialize()
        {
            // Initialize base
            await base.Initialize();
            
            // Initialise variables with default value
            ActiveStateBudget = new StateBudgetEntity();
            StateBudgets = new List<StateBudgetEntity>();
            
            // Populate based on facts
            StateBudgets = await UnitOfWork.StateBudgets.GetAllAsync();
            ActiveStateBudget = UnitOfWork.StateBudgets.GetActiveFromList(StateBudgets);
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

        public async Task OnPost(StateBudgetEntity model)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();

                if (model.Id == Guid.Empty)
                {
                    throw new BadRequestException("Data APBN tidak valid.");
                }

                var stateBudget = await UnitOfWork.StateBudgets.GetByIdAsync(model.Id);
                if (stateBudget is null)
                {
                    throw new BadRequestException("Kebijakan APBN tersebut tidak ditemukan.");
                }

                if (model.CountryIncome <= 0)
                {
                    throw new BadRequestException("Pendapatan Negara harus lebih dari Rp 0.");
                }

                stateBudget.CountryIncome = model.CountryIncome;
                await UnitOfWork.StateBudgets.ModifyAsync(stateBudget);

                await Initialize();
            await UnitOfWork.CommitAsync();
                SetSuccessAlert($"Pendapatan Negara dalam {Formatter.GetStateBudgetPolicyName(stateBudget)} " +
                                $"telah diatur menjadi Rp {stateBudget.CountryIncome} T.");
            }
            catch (Exception e)
            {
                await UnitOfWork.RollbackAsync();
                SetErrorAlert(e.Message);
                if (!(e is GenericException)) throw;
            }
        }
    }
}
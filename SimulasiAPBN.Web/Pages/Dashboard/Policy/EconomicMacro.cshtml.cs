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
    public class EconomicMacro : BasePage
    {
        private readonly IValidatorFactory _validatorFactory;

        public EconomicMacro(
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

        public Core.Models.StateBudget StateBudget { get; private set; }
        public ICollection<Core.Models.StateBudget> StateBudgets { get; private set; }
        public ICollection<Core.Models.EconomicMacro> EconomicMacros { get; private set; }
        
        public const string AddEconomicMacroAction = "AddEconomicMacroAction";
        public const string ModifyEconomicMacroAction = "ModifyEconomicMacroAction";
        public const string RemoveEconomicMacroAction = "RemoveEconomicMacroAction";

        protected override async Task Initialize()
        {
            // Populate base
            await base.Initialize();

            // Initialise variables with default value
            StateBudget = new Core.Models.StateBudget();
            StateBudgets = new List<Core.Models.StateBudget>();
            EconomicMacros = new List<Core.Models.EconomicMacro>();

            // Populate based on facts

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

            EconomicMacros = (await UnitOfWork.EconomicMacros.GetAllAsync())
                .OrderBy(economicMacro => economicMacro.Name)
                .ToList();
            
            if (!EconomicMacros.Any())
            {
                return;
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

        public async Task OnPost([FromForm] string action, Core.Models.EconomicMacro model)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();
                if (string.IsNullOrEmpty(action)) return;
                switch (action)
                {
                    case {} a when a == AddEconomicMacroAction:
                        await AddEconomicMacro(model);
                        break;
                    case {} a when a == ModifyEconomicMacroAction:
                        await ModifyEconomicMacro(model);
                        break;
                    case {} a when a == RemoveEconomicMacroAction:
                        await RemoveEconomicMacro(model);
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

        private async Task AddEconomicMacro(Core.Models.EconomicMacro model)
        {
            (await _validatorFactory.EconomicMacro.GetValidationAsync(model))
                .ThrowIfInvalid();

            var economicMacro = new Core.Models.EconomicMacro
            {
                StateBudget = StateBudget,
                Name = model.Name,
                DefaultValue = model.DefaultValue,
                MinimumValue = model.MinimumValue,
                MaximumValue = model.MaximumValue,
                Threshold = model.Threshold,
                ThresholdValue = model.ThresholdValue,
                OrderFlag = model.OrderFlag,
                UnitDesc = model.UnitDesc
            };

            await UnitOfWork.EconomicMacros.AddAsync(economicMacro);

            foreach (var economics in EconomicMacros)
            {
                if (economics.Id != model.Id)
                {
                    if (economics.OrderFlag >= model.OrderFlag)
                    {
                        economics.OrderFlag += 1;
                        await UnitOfWork.EconomicMacros.ModifyAsync(economics);
                    }
                }
            }

            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Asumsi {economicMacro.Name} telah ditambahkan.");
        }

        private async Task ModifyEconomicMacro(Core.Models.EconomicMacro model)
        {
            (await _validatorFactory.EconomicMacro.GetValidationAsync(model))
                .ThrowIfInvalid();
                        
            var economicMacro = await UnitOfWork.EconomicMacros
                .GetByIdAsync(model.Id);
            if (economicMacro is null)
            {
                throw new BadRequestException("Asumsi Ekonomi tersebut tidak ditemukan.");
            }

            foreach (var economics in EconomicMacros)
            {
                if (economics.Id != model.Id)
                {
                    if (economics.OrderFlag == model.OrderFlag)
                    {
                        economics.OrderFlag = economicMacro.OrderFlag;
                        await UnitOfWork.EconomicMacros.ModifyAsync(economics);
                    }
                }
            }

            economicMacro.Name = model.Name;
            economicMacro.DefaultValue = model.DefaultValue;
            economicMacro.MinimumValue = model.MinimumValue;
            economicMacro.MaximumValue = model.MaximumValue;
            economicMacro.Threshold = model.Threshold;
            economicMacro.ThresholdValue = model.ThresholdValue;
            economicMacro.OrderFlag = model.OrderFlag;
            economicMacro.UnitDesc = model.UnitDesc;

            await UnitOfWork.EconomicMacros.ModifyAsync(economicMacro);

            await Initialize();
            await UnitOfWork.CommitAsync();

            SetSuccessAlert($"Asumsi {economicMacro.Name} telah berhasil diubah.");
        }

        private async Task RemoveEconomicMacro(Core.Models.EconomicMacro model)
        {
            var economicMacro = await UnitOfWork.EconomicMacros
                .GetByIdAsync(model.Id);
            if (economicMacro is null)
            {
                throw new BadRequestException("Asumsi Ekonomi tersebut tidak ditemukan.");
            }

            await UnitOfWork.EconomicMacros.RemoveAsync(economicMacro);

            await Initialize();
            await UnitOfWork.CommitAsync();

            SetSuccessAlert($"Asumsi {economicMacro.Name} telah berhasil dihapus.");
        }
    }
}
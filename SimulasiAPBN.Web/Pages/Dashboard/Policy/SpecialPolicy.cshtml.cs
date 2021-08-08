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
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Validation;

namespace SimulasiAPBN.Web.Pages.Dashboard.Policy
{
    public class SpecialPolicy : BasePage
    {
        private readonly IValidatorFactory _validatorFactory;

        public SpecialPolicy(
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
        
        public IEnumerable<Core.Models.SpecialPolicy> SpecialPolicies { get; private set; }
        public Core.Models.StateBudget StateBudget { get; private set; }
        public IEnumerable<Core.Models.StateBudget> StateBudgets { get; private set; }

        public const string AddSpecialPolicyAction = "AddSpecialPolicyAction";
        public const string ModifySpecialPolicyAction = "ModifySpecialPolicyAction";
        public const string RemoveSpecialPolicyAction = "RemoveSpecialPolicyAction";
        
        protected override async Task Initialize()
        {
            // Initialize base
            await base.Initialize();
            
            // Initialise variables with default value
            StateBudget = new Core.Models.StateBudget();
            StateBudgets = new List<Core.Models.StateBudget>();
            SpecialPolicies = new List<Core.Models.SpecialPolicy>();
            
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

            SpecialPolicies = StateBudget.SpecialPolicies;
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

        public async Task OnPost([FromForm] string action, Core.Models.SpecialPolicy model)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();
                if (string.IsNullOrEmpty(action)) return;
                switch (action)
                {
                    case {} a when a == AddSpecialPolicyAction:
                        await AddSpecialPolicy(model);
                        break;
                    case {} a when a == ModifySpecialPolicyAction:
                        await ModifySpecialPolicy(model);
                        break;
                    case {} a when a == RemoveSpecialPolicyAction:
                        await RemoveSpecialPolicy(model);
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

        private async Task AddSpecialPolicy(Core.Models.SpecialPolicy model)
        {
            (await _validatorFactory.SpecialPolicy.GetValidationAsync(model))
                .ThrowIfInvalid();

            var specialPolicy = new Core.Models.SpecialPolicy
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive,
                TotalAllocation = model.TotalAllocation,
                StateBudget = StateBudget,
            };
            await UnitOfWork.SpecialPolicies.AddAsync(specialPolicy);
			
            await Initialize();
            await UnitOfWork.CommitAsync();
			
            SetSuccessAlert($"Kebijakan Khusus {specialPolicy.Name} telah berhasil ditambahkan.");
        }
        
        private async Task ModifySpecialPolicy(Core.Models.SpecialPolicy model)
        {
            (await _validatorFactory.SpecialPolicy.GetValidationAsync(model))
                .ThrowIfInvalid();
            
            var specialPolicy = await UnitOfWork.SpecialPolicies
                .GetByIdAsync(model.Id);
            if (specialPolicy is null)
            {
                throw new BadRequestException("Kebijakan Khusus tersebut tidak ditemukan.");
            }

            specialPolicy.Name = model.Name;
            specialPolicy.Description = model.Description;
            specialPolicy.IsActive = model.IsActive;
            specialPolicy.TotalAllocation = model.TotalAllocation;
            await UnitOfWork.SpecialPolicies.ModifyAsync(specialPolicy);
			
            await Initialize();
            await UnitOfWork.CommitAsync();
			
            SetSuccessAlert($"Kebijakan Khusus {specialPolicy.Name} telah berhasil diubah.");
        }
        
        private async Task RemoveSpecialPolicy(Core.Models.SpecialPolicy model)
        {
            var specialPolicy = await UnitOfWork.SpecialPolicies
                .GetByIdAsync(model.Id);
            if (specialPolicy is null)
            {
                throw new BadRequestException("Kebijakan Khusus tersebut tidak ditemukan.");
            }

            await UnitOfWork.SpecialPolicyAllocations.RemoveRangeAsync(specialPolicy.SpecialPolicyAllocations);
            await UnitOfWork.SpecialPolicies.RemoveAsync(specialPolicy);
			
            await Initialize();
            await UnitOfWork.CommitAsync();
			
            SetSuccessAlert($"Kebijakan Khusus {specialPolicy.Name} telah berhasil dihapus.");
        }
    }
}
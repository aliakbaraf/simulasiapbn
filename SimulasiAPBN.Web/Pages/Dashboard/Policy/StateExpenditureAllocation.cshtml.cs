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
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Validation;
using AllocationEntity = SimulasiAPBN.Core.Models.Allocation;
using StateBudgetEntity = SimulasiAPBN.Core.Models.StateBudget;
using StateExpenditureEntity = SimulasiAPBN.Core.Models.StateExpenditure;
using StateExpenditureAllocationEntity = SimulasiAPBN.Core.Models.StateExpenditureAllocation;

namespace SimulasiAPBN.Web.Pages.Dashboard.Policy
{
    public class StateExpenditureAllocation : BasePage
    {
        private readonly IValidatorFactory _validatorFactory;

        public StateExpenditureAllocation(
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
        public string StateExpenditureId { get; set; }
        
        public ICollection<AllocationEntity> Allocations { get; private set; }
        public ICollection<AllocationEntity> AvailableAllocations { get; private set; }
        public StateBudgetEntity StateBudget { get; set; }
        public StateExpenditureEntity StateExpenditure { get; private set; }
        public ICollection<StateExpenditureAllocationEntity> StateExpenditureAllocations { get; private set; }
        public string BudgetName { get; private set; }
        public string BudgetType { get; private set; }
        private string StateBudgetName { get; set; }
        public decimal AvailableAllocationValue { get; private set; }
        public decimal UsedAllocationValue { get; private set; }

        public const string AddStateExpenditureAllocationAction = "AddStateExpenditureAllocationAction";
        public const string ModifyStateExpenditureAllocationAction = "ModifyStateExpenditureAllocationAction";
        public const string RemoveStateExpenditureAllocationAction = "RemoveStateExpenditureAllocationAction";

        protected override async Task Initialize()
        {
            // Initialize base
            await base.Initialize();
            
            // Initialise variables with default value
            Allocations = new List<AllocationEntity>();
            AvailableAllocations = new List<AllocationEntity>();
            AvailableAllocationValue = 0;
            UsedAllocationValue = 0;
            BudgetName = "Anggaran";
            StateBudget = new StateBudgetEntity();
            StateExpenditure = new StateExpenditureEntity();
            StateExpenditureAllocations = new List<StateExpenditureAllocationEntity>();

            // Dependencies checking
            if (string.IsNullOrEmpty(StateExpenditureId) || 
                !Guid.TryParse(StateExpenditureId, out var stateExpenditureGuid))
            {
                throw new BadRequestException("Anggaran Belanja Negara tersebut tidak ditemukan.");
            }
            
            // Populate based on facts
            Allocations = (await UnitOfWork.Allocations.GetAllAsync())
                .OrderBy(allocation => allocation.CreatedAt)
                .ToList();
            
            StateExpenditure =  await UnitOfWork.StateExpenditures.GetByIdAsync(stateExpenditureGuid);
            if (StateExpenditure is null)
            {
                throw new BadRequestException("Anggaran Belanja Negara tersebut tidak ditemukan.");
            }

            var stateExpenditureAllocations = await UnitOfWork.StateExpenditureAllocations
                .GetByStateExpenditureAsync(StateExpenditure);
            StateExpenditureAllocations = stateExpenditureAllocations.ToList();
            UsedAllocationValue = StateExpenditureAllocations
                .Sum(expenditureAllocation => expenditureAllocation.TotalAllocation);
            AvailableAllocationValue = StateExpenditure.TotalAllocation - UsedAllocationValue;
            
            StateBudget = await UnitOfWork.StateBudgets.GetByIdAsync(StateExpenditure.StateBudgetId);
            if (StateExpenditure is null)
            {
                throw new BadRequestException("Kebijakan APBN tersebut tidak ditemukan.");
            }

            StateBudgetName = Formatter.GetStateBudgetPolicyName(StateBudget);
            BudgetName = $"Anggaran {StateExpenditure.Budget.Function} " +
                         $"({StateBudgetName})";
            BudgetType = Formatter.GetBudgetTypeName(StateExpenditure.Budget.Type);

            foreach (var allocation in Allocations)
            {
                if (StateExpenditureAllocations.All(expenditureAllocation => 
                    expenditureAllocation.AllocationId != allocation.Id))
                {
                    AvailableAllocations.Add(allocation);
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

        public async Task OnPost([FromForm] string action, StateExpenditureAllocationEntity model)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();
                if (string.IsNullOrEmpty(action)) return;
                switch (action)
                {
                    case { } a when a == AddStateExpenditureAllocationAction:
                        await AddStateExpenditureAllocation(model);
                        break;
                    case { } a when a == ModifyStateExpenditureAllocationAction:
                        await ModifyStateExpenditureAllocation(model);
                        break;
                    case { } a when a == RemoveStateExpenditureAllocationAction:
                        await RemoveStateExpenditureAllocation(model);
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

        private async Task AddStateExpenditureAllocation(StateExpenditureAllocationEntity model)
        {
            (await _validatorFactory.StateExpenditureAllocation.GetValidationAsync(model))
                .ThrowIfInvalid();
            
            var allocation = await UnitOfWork.Allocations.GetByIdAsync(model.AllocationId);
            if (allocation is null)
            {
                throw new BadRequestException("Data Alokasi yang Anda pilih tidak ditemukan.");
            }

            if (model.TotalAllocation > StateExpenditure.TotalAllocation || 
                model.TotalAllocation > AvailableAllocationValue)
            {
                throw new BadRequestException(
                    $"Nilai Total Alokasi {allocation.Name} (Rp {model.TotalAllocation} T) melebihi Total " +
                    $"Alokasi Anggaran {StateExpenditure.Budget.Function} (total alokasi " +
                    $"Rp {StateExpenditure.TotalAllocation} T, tersedia Rp {AvailableAllocationValue} T).");
            }

            var stateExpenditureAllocation = new StateExpenditureAllocationEntity
            {
                Allocation = allocation,
                StateExpenditure = StateExpenditure,
                TotalAllocation = model.TotalAllocation,
                Percentage = (model.TotalAllocation / StateExpenditure.TotalAllocation) * 100
            };
            await UnitOfWork.StateExpenditureAllocations.AddAsync(stateExpenditureAllocation);
            
            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Alokasi {allocation.Name} telah dianggarkan sebesar " +
                            $"Rp {model.TotalAllocation} T dari Anggaran " +
                            $"{StateExpenditure.Budget.Function}.");
        }

        private async Task ModifyStateExpenditureAllocation(StateExpenditureAllocationEntity model)
        {
            (await _validatorFactory.StateExpenditureAllocation.GetValidationAsync(model))
                .ThrowIfInvalid();

            var stateExpenditureAllocation = await UnitOfWork.StateExpenditureAllocations.GetByIdAsync(model.Id);
            if (stateExpenditureAllocation is null)
            {
                throw new BadRequestException(
                    "Data Alokasi Anggaran yang Anda pilih tidak ditemukan.");
            }
            var allocation = await UnitOfWork.Allocations.GetByIdAsync(model.AllocationId);
            if (allocation is null)
            {
                throw new BadRequestException("Data Alokasi yang Anda pilih tidak ditemukan.");
            }

            stateExpenditureAllocation.Allocation = allocation;
            stateExpenditureAllocation.TotalAllocation = model.TotalAllocation;
            stateExpenditureAllocation.Percentage = (model.TotalAllocation / StateExpenditure.TotalAllocation) * 100;
            await UnitOfWork.StateExpenditureAllocations.ModifyAsync(stateExpenditureAllocation);

            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Alokasi {allocation.Name} telah dianggarkan sebesar " +
                            $"Rp {model.TotalAllocation} T dari Anggaran " +
                            $"{StateExpenditure.Budget.Function}.");
        }

        public async Task RemoveStateExpenditureAllocation(StateExpenditureAllocationEntity model)
        {
            var stateExpenditureAllocation = await UnitOfWork.StateExpenditureAllocations.GetByIdAsync(model.Id);
            if (stateExpenditureAllocation is null)
            {
                throw new BadRequestException(
                    "Data Alokasi Anggaran yang Anda pilih tidak ditemukan.");
            }
            var allocationName = stateExpenditureAllocation.Allocation.Name;
            await UnitOfWork.StateExpenditureAllocations.RemoveAsync(stateExpenditureAllocation);
            
            var stateExpenditureAllocations = new List<StateExpenditureAllocationEntity>
            {
                stateExpenditureAllocation
            };
            StateExpenditure.StateExpenditureAllocations = StateExpenditure.StateExpenditureAllocations
                .Except(stateExpenditureAllocations)
                .ToList();

            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Alokasi {allocationName} telah dihapus dari Anggaran " +
                            $"{StateExpenditure.Budget.Function}.");
        }
    }
}
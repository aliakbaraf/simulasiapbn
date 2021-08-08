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
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Validation;
using AllocationEntity = SimulasiAPBN.Core.Models.Allocation;

namespace SimulasiAPBN.Web.Pages.Dashboard.Budgeting
{
    public class Allocation : BasePage
    {
        private readonly IValidatorFactory _validatorFactory;

        public Allocation(
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
        
        public IEnumerable<AllocationEntity> Allocations { get; set; }

        public const string AddAllocationAction = "AddAllocationAction";
        public const string ModifyAllocationAction = "ModifyAllocationAction";
        public const string RemoveAllocationAction = "RemoveAllocationAction";

        protected override async Task Initialize()
        {
            // Initialize base
            await base.Initialize();
            
            // Initialise variables with default value
            Allocations = new List<AllocationEntity>();
            
            // Populate based on facts
            Allocations = (await UnitOfWork.Allocations.GetAllAsync())
                .OrderByDescending(allocation => allocation.IsMandatory)
                .ThenBy(allocation => allocation.Name)
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

        public async Task OnPost([FromForm] string action, AllocationEntity model)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();
                if (string.IsNullOrEmpty(action)) return;
                switch (action)
                {
                    case {} a when a == AddAllocationAction:
                        await AddAllocation(model);
                        break;
                    case {} a when a == ModifyAllocationAction:
                        await ModifyAllocation(model);
                        break;
                    case {} a when a == RemoveAllocationAction:
                        await RemoveAllocation(model);
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

        private static void ValidateAllocationMandatory(AllocationEntity model)
        {
            if (!model.IsMandatory) return;
            if (string.IsNullOrEmpty(model.MandatoryExplanation))
            {
                throw new BadRequestException(
                    "Penjelasan Alokasi Wajib tidak boleh kosong.");
            }
            if (model.MandatoryThreshold <= 0)
            {
                throw new BadRequestException(
                    "Ambang Batas Minimum tidak boleh kosong atau nol.");
            }
            if (model.MandatoryThreshold > 100)
            {
                throw new BadRequestException(
                    "Ambang Batas Minimum tidak boleh lebih dari 100%.");
            }
        }

        private async Task AddAllocation(AllocationEntity model)
        {
            
            (await _validatorFactory.Allocation.GetValidationAsync(model))
                .ThrowIfInvalid();

            var allocation = await UnitOfWork.Allocations
                .FindOneAsync(entity => entity.Name == model.Name);
            if (allocation is not null)
            {
                throw new BadRequestException(
                    $"Alokasi {allocation.Name} telah tercatat sebelumnya, silakan " +
                    $"gunakan nama alokasi yang lain.");
            }

            ValidateAllocationMandatory(model);

            allocation = new AllocationEntity
            {
                Name = model.Name,
                IsMandatory = model.IsMandatory,
                MandatoryExplanation = model.MandatoryExplanation,
                MandatoryThreshold = model.MandatoryThreshold
            };
            await UnitOfWork.Allocations.AddAsync(allocation);

            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Alokasi {allocation.Name} telah ditambahkan.");
        }

        private async Task ModifyAllocation(AllocationEntity model)
        {
            var allocation = await UnitOfWork.Allocations.GetByIdAsync(model.Id);
            if (allocation is null)
            {
                throw new BadRequestException("Alokasi Anggaran tersebut tidak ditemukan.");
            }

            ValidateAllocationMandatory(model);
            
            allocation.Name = model.Name;
            allocation.IsMandatory = model.IsMandatory;
            allocation.MandatoryExplanation = model.MandatoryExplanation;
            allocation.MandatoryThreshold = model.MandatoryThreshold;
            await UnitOfWork.Allocations.ModifyAsync(allocation);

            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Detil Alokasi {allocation.Name} telah diubah.");
        }
        
        private async Task RemoveAllocation(GenericModel model)
        {
            var allocation =  await UnitOfWork.Allocations.GetByIdAsync(model.Id);
            if (allocation is null)
            {
                throw new BadRequestException("Alokasi Anggaran tersebut tidak ditemukan.");
            }

            var stateExpenditureAllocations = (await UnitOfWork
                    .StateExpenditureAllocations
                    .GetByAllocationAsync(allocation))
                .ToList();
            if (stateExpenditureAllocations.Any())
            {
                throw new BadRequestException(
                    $"Tidak dapat menghapus Alokasi {allocation.Name}. Alokasi {allocation.Name} masih " +
                    $"terkait dengan salah satu Belanja Negara. Silakan hapus Alokasi {allocation.Name} dari seluruh " +
                    "Belanja Negara terlebih dahulu.");
            }
            
            var specialPolicyAllocation = (await UnitOfWork
                    .SpecialPolicyAllocations
                    .GetByAllocationAsync(allocation))
                .ToList();
            if (specialPolicyAllocation.Any())
            {
                throw new BadRequestException(
                    $"Tidak dapat menghapus Alokasi {allocation.Name}. Alokasi {allocation.Name} masih " +
                    $"terkait dengan salah satu Kebijakan Khusus. Silakan hapus Alokasi {allocation.Name} dari seluruh " +
                    "Kebijakan Khusus terlebih dahulu.");
            }
            
            await UnitOfWork.Allocations.RemoveAsync(allocation);

            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert($"Alokasi {allocation.Name} telah dihapus.");
        }
        
    }
}
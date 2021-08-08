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

namespace SimulasiAPBN.Web.Pages.Dashboard.Policy
{
	public class SpecialPolicyAllocation : BasePage
	{
		private readonly IValidatorFactory _validatorFactory;
		
		public SpecialPolicyAllocation(
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
		
		public ICollection<Allocation> Allocations { get; private set; }
		public ICollection<Allocation> AvailableAllocations { get; private set; }
		[BindProperty(SupportsGet = true)]
		// ReSharper disable once UnusedAutoPropertyAccessor.Global MemberCanBePrivate.Global
		public string SpecialPolicyId { get; set; }
		public Core.Models.SpecialPolicy SpecialPolicy { get; private set; }
		public string SpecialPolicyName { get; private set; }
		public decimal AvailableAllocationValue { get; private set; }
		public decimal UsedAllocationValue { get; private set; }
		
		public const string AddSpecialPolicyAllocationAction = "AddSpecialPolicyAllocationAction";
		public const string ModifySpecialPolicyAllocationAction = "ModifySpecialPolicyAllocationAction";
		public const string RemoveSpecialPolicyAllocationAction = "RemoveSpecialPolicyAllocationAction";

		protected override async Task Initialize()
		{
			// Initialize base
			await base.Initialize();
            
			// Initialise variables with default value
			Allocations = new Collection<Allocation>();
			AvailableAllocations = new Collection<Allocation>();
			AvailableAllocationValue = 0;
			SpecialPolicy = new Core.Models.SpecialPolicy();
			SpecialPolicyName = "Kebijakan Khusus";
			UsedAllocationValue = 0;

			// Populate based on facts
			if (string.IsNullOrEmpty(SpecialPolicyId))
			{
				throw new BadRequestException("Mohon pilih Kebijakan Khusus.");
			}
			
			if (!Guid.TryParse(SpecialPolicyId, out var specialPolicyGuid))
			{
				throw new BadRequestException("Kebijakan Khusus tidak valid.");
			}
			
			SpecialPolicy = await UnitOfWork.SpecialPolicies.GetByIdAsync(specialPolicyGuid);
			if (SpecialPolicy is null)
			{
				SpecialPolicy = new Core.Models.SpecialPolicy();
				throw new BadRequestException(
					"Data Kebijakan Khusus yang Anda pilih tidak ditemukan.");
			}

			SpecialPolicyName = SpecialPolicy.Name;
			
			Allocations = (await UnitOfWork.Allocations.GetAllAsync())
				.ToList();
			foreach (var allocation in Allocations)
			{
				var allocationIsAvailable = SpecialPolicy.SpecialPolicyAllocations
					.All(specialPolicyAllocation => specialPolicyAllocation.AllocationId != allocation.Id);
				if (allocationIsAvailable)
				{
					AvailableAllocations.Add(allocation);
				}
			}
			
			UsedAllocationValue = SpecialPolicy.SpecialPolicyAllocations
				.Sum(policyAllocation => policyAllocation.TotalAllocation);
			AvailableAllocationValue = SpecialPolicy.TotalAllocation - UsedAllocationValue;
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
		
		public async Task OnPost([FromForm] string action, Core.Models.SpecialPolicyAllocation model)
		{
			UnitOfWork.BeginTransaction();
			try
			{
				await Initialize();
				if (string.IsNullOrEmpty(action)) return;
				switch (action)
				{
					case { } a when a == AddSpecialPolicyAllocationAction:
						await AddSpecialPolicyAllocation(model);
						break;
					case { } a when a == ModifySpecialPolicyAllocationAction:
						await ModifySpecialPolicyAllocation(model);
						break;
					case { } a when a == RemoveSpecialPolicyAllocationAction:
						await RemoveSpecialPolicyAllocation(model);
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

		private async Task AddSpecialPolicyAllocation(Core.Models.SpecialPolicyAllocation model)
		{
			(await _validatorFactory.SpecialPolicyAllocation.GetValidationAsync(model))
				.ThrowIfInvalid();
			
			var allocation = await UnitOfWork.Allocations.GetByIdAsync(model.AllocationId);
			if (allocation is null)
			{
				throw new BadRequestException("Data Alokasi Anggaran yang Anda pilih tidak ditemukan.");
			}

			if (model.TotalAllocation > SpecialPolicy.TotalAllocation || 
			    model.TotalAllocation > AvailableAllocationValue)
			{
				throw new BadRequestException(
					$"Nilai Total Alokasi {allocation.Name} (Rp {model.TotalAllocation} T) melebihi Total " +
					$"Alokasi {SpecialPolicyName} (total alokasi Rp {SpecialPolicy.TotalAllocation} T, tersedia " +
					$"Rp {AvailableAllocationValue} T).");
			}

			var specialPolicyAllocation = new Core.Models.SpecialPolicyAllocation
			{
				Allocation = allocation,
				SpecialPolicy = SpecialPolicy,
				SimulationMaximumMultiplier = model.SimulationMaximumMultiplier / 100,
				TotalAllocation = model.TotalAllocation,
				Percentage = (model.TotalAllocation / SpecialPolicy.TotalAllocation) * 100
			};
			await UnitOfWork.SpecialPolicyAllocations.AddAsync(specialPolicyAllocation);
            
			await Initialize();
			await UnitOfWork.CommitAsync();
			SetSuccessAlert($"Alokasi {allocation.Name} telah dianggarkan sebesar " +
			                $"Rp {model.TotalAllocation} T dari {SpecialPolicyName}.");
		}

		private async Task ModifySpecialPolicyAllocation(Core.Models.SpecialPolicyAllocation model)
		{
			(await _validatorFactory.SpecialPolicyAllocation.GetValidationAsync(model))
				.ThrowIfInvalid();
			
			var specialPolicyAllocation = await UnitOfWork.SpecialPolicyAllocations.GetByIdAsync(model.Id);
			if (specialPolicyAllocation is null)
			{
				throw new BadRequestException(
					$"Data Alokasi {SpecialPolicyName} yang Anda pilih tidak ditemukan.");
			}
			
			var allocation = await UnitOfWork.Allocations.GetByIdAsync(model.AllocationId);
			if (allocation is null)
			{
				throw new BadRequestException("Data Alokasi Anggaran yang Anda pilih tidak ditemukan.");
			}

			specialPolicyAllocation.Allocation = allocation;
			specialPolicyAllocation.SimulationMaximumMultiplier = model.SimulationMaximumMultiplier / 100;
			specialPolicyAllocation.TotalAllocation = model.TotalAllocation;
			specialPolicyAllocation.Percentage = (model.TotalAllocation / SpecialPolicy.TotalAllocation) * 100;
			await UnitOfWork.SpecialPolicyAllocations.ModifyAsync(specialPolicyAllocation);

			await Initialize();
			await UnitOfWork.CommitAsync();
			SetSuccessAlert($"Alokasi {allocation.Name} telah dianggarkan sebesar " +
			                $"Rp {model.TotalAllocation} T dari {SpecialPolicyName}.");
		}

		private async Task RemoveSpecialPolicyAllocation(GenericModel model)
		{
			var specialPolicyAllocation = await UnitOfWork.SpecialPolicyAllocations.GetByIdAsync(model.Id);
			if (specialPolicyAllocation is null)
			{
				throw new BadRequestException(
					$"Data Alokasi {SpecialPolicyName} yang Anda pilih tidak ditemukan.");
			}

			var allocationName = specialPolicyAllocation.Allocation.Name;
			await UnitOfWork.SpecialPolicyAllocations.RemoveAsync(specialPolicyAllocation);
			
			await Initialize();
			await UnitOfWork.CommitAsync();
			
			SetSuccessAlert($"Alokasi {allocationName} telah dihapus dari {SpecialPolicyName}.");
		}
	}
}
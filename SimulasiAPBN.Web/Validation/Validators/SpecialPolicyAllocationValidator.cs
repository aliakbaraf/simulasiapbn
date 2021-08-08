/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using FluentValidation;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Validation.Common;

namespace SimulasiAPBN.Web.Validation.Validators
{
	public class SpecialPolicyAllocationValidator : Validator<SpecialPolicyAllocation>
	{
		public SpecialPolicyAllocationValidator()
		{
			RuleFor(model => model.AllocationId)
				.NotNull()
				.WithMessage("Silakan pilih Alokasi Anggaran.");
			RuleFor(model => model.AllocationId)
				.NotEmpty()
				.WithMessage("Silakan pilih Alokasi Anggaran.");
			RuleFor(model => model.SimulationMaximumMultiplier)
				.GreaterThanOrEqualTo(100)
				.WithMessage("Simulasi Maksimum harus lebih dari 100% alokasi anggaran.");
			RuleFor(model => model.SimulationMaximumMultiplier)
				.LessThanOrEqualTo(1000)
				.WithMessage("Simulasi Maksimum harus kurang dari 1000% alokasi anggaran.");
			RuleFor(model => model.TotalAllocation)
				.NotNull()
				.WithMessage("Silakan masukan Total Alokasi.");
			RuleFor(model => model.TotalAllocation)
				.NotEmpty()
				.WithMessage("Silakan masukan Total Alokasi.");
			RuleFor(model => model.TotalAllocation)
				.GreaterThan(0)
				.WithMessage("Total Alokasi harus lebih dari Rp 0.");
		}
	}
}
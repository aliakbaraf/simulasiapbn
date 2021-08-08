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
	public class SpecialPolicyValidator : Validator<SpecialPolicy>
	{
		public SpecialPolicyValidator()
		{
			RuleFor(model => model.Name)
				.NotNull()
				.WithMessage("Silakan masukan Nama Kebijakan Khusus.");
			RuleFor(model => model.Name)
				.NotEmpty()
				.WithMessage("Silakan masukan Nama Kebijakan Khusus.");
			RuleFor(model => model.Description)
				.NotNull()
				.WithMessage("Silakan masukan Deskripsi Kebijakan Khusus.");
			RuleFor(model => model.Description)
				.NotEmpty()
				.WithMessage("Silakan masukan Deskripsi Kebijakan Khusus.");
			RuleFor(model => model.TotalAllocation)
				.NotEmpty()
				.WithMessage("Silakan masukan Total Alokasi Kebijakan Khusus.");
			RuleFor(model => model.TotalAllocation)
				.GreaterThan(0)
				.WithMessage("Total Alokasi Kebijakan Khusus harus lebih dari Rp 0.");
		}
	}
}
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
    public class StateExpenditureAllocationValidator : Validator<StateExpenditureAllocation>
    {
        public StateExpenditureAllocationValidator()
        {
            RuleFor(model => model.AllocationId)
                .NotNull()
                .WithMessage("Silakan pilih Alokasi Anggaran.");
            RuleFor(model => model.AllocationId)
                .NotEmpty()
                .WithMessage("Silakan pilih Alokasi Anggaran.");
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
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
    public class AllocationValidator : Validator<Allocation>
    {
        public AllocationValidator()
        {
            RuleFor(model => model.Name)
                .NotNull()
                .WithMessage("Silakan masukan Nama Alokasi.");
            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Silakan masukan Nama Alokasi.");
        }
    }
}
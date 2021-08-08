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
    public class BudgetValidator : Validator<Budget>
    {
        public BudgetValidator()
        {
            RuleFor(model => model.Function)
                .NotNull()
                .WithMessage("Silakan masukan Fungsi Anggaran.");
            RuleFor(model => model.Function)
                .NotEmpty()
                .WithMessage("Silakan masukan Fungsi Anggaran.");
            RuleFor(model => model.Type)
                .NotNull()
                .WithMessage("Silakan pilih Jenis Anggaran.");
            RuleFor(model => model.Type)
                .IsInEnum()
                .WithMessage("Jenis Anggaran yang Anda pilih tidak tersedia.");
        }
    }
}
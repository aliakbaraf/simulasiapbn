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
    public class StateBudgetValidator : Validator<StateBudget>
    {
        public StateBudgetValidator()
        {
            RuleFor(model => model.Year)
                .NotNull()
                .WithMessage("Silakan pilih Periode APBN.");
            RuleFor(model => model.Year)
                .NotEmpty()
                .WithMessage("Silakan pilih Periode APBN.");
        }
    }
}
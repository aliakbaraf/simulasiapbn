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
    public class EconomicMacroValidator : Validator<EconomicMacro>
    {
        public EconomicMacroValidator()
        {
            RuleFor(model => model.Name)
                .NotNull()
                .WithMessage("Silakan masukan Nama Asumsi Ekonomi.");
            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Silakan masukan Nama Asumsi Ekonomi.");
        }
    }
}
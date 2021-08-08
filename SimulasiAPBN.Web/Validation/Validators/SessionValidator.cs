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
    public class SessionValidator : Validator<SimulationSession>
    {
        public SessionValidator()
        {
            RuleFor(model => model.Name)
                .NotNull()
                .WithMessage("Silakan masukan Nama Anda.");
            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Silakan masukan Nama Anda.");
            // RuleFor(session => session.StateBudget).NotNull();
        }
    }
}
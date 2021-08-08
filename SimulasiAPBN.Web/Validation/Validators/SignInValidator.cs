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
    public class SignInValidator : Validator<Account>
    {
        public SignInValidator()
        {
            RuleFor(model => model.Username)
                .NotNull()
                .WithMessage("Silakan masukan Nama Pengguna Anda.");
            RuleFor(model => model.Username)
                .NotEmpty()
                .WithMessage("Silakan masukan Nama Pengguna Anda.");
            RuleFor(model => model.Password)
                .NotNull()
                .WithMessage("Silakan masukan Kata Sandi Anda.");
            RuleFor(model => model.Password)
                .NotEmpty()
                .WithMessage("Silakan masukan Kata Sandi Anda.");
        }
    }
}
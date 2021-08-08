/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Text.RegularExpressions;
using FluentValidation;
using SimulasiAPBN.Web.Models;
using SimulasiAPBN.Web.Validation.Common;

namespace SimulasiAPBN.Web.Validation.Validators
{
    public class ChangePasswordValidator : Validator<ChangePassword>
    {

        public ChangePasswordValidator()
        {
            /* Old Password Testing */
            RuleFor(model => model.OldPassword)
                .NotNull()
                .WithMessage("Silakan masukan Kata Sandi lama Anda.");
            RuleFor(model => model.OldPassword)
                .NotEmpty()
                .WithMessage("Silakan masukan Kata Sandi lama Anda.");

            /* New Password Testing */
            RuleFor(model => model.NewPassword)
                .NotNull()
                .WithMessage("Silakan masukan Kata Sandi baru Anda.");
            RuleFor(model => model.NewPassword)
                .NotEmpty()
                .WithMessage("Silakan masukan Kata Sandi baru Anda.");
            RuleFor(model => model.NewPassword)
                .MinimumLength(8)
                .WithMessage("Kata Sandi minimum terdiri dari 8 karakter.");
            RuleFor(model => model.NewPassword)
                .Matches(new Regex("[A-Z]"))
                .WithMessage("Kata Sandi wajib mengandung huruf kapital (A-Z).");
            RuleFor(model => model.NewPassword)
                .Matches(new Regex("[a-z]"))
                .WithMessage("Kata Sandi wajib mengandung huruf non-kapital (a-z).");
            RuleFor(model => model.NewPassword)
                .Matches(new Regex("[0-9]"))
                .WithMessage("Kata Sandi baru wajib mengandung angka (0-9).");

            /* New Password Confirmation Testing */
            RuleFor(model => model.NewPasswordConfirmation)
                .NotNull()
                .WithMessage("Silakan masukan konfirmasi Kata Sandi baru Anda.");
            RuleFor(model => model.NewPasswordConfirmation)
                .NotEmpty()
                .WithMessage("Silakan masukan konfirmasi Kata Sandi baru Anda.");
            RuleFor(model => model.NewPasswordConfirmation)
                .Equal(model => model.NewPassword)
                .WithMessage("Kata Sandi baru tidak sesuai dengan konfirmasi.");
            
        }
        
    }
}
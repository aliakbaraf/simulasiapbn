/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Text.RegularExpressions;
using FluentValidation;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Validation.Common;

namespace SimulasiAPBN.Web.Validation.Validators
{
    public class UpsertAnalystValidator : Validator<Account>
    {
        public UpsertAnalystValidator()
        {
            /* Name Testing */
            RuleFor(model => model.Name)
                .NotNull()
                .WithMessage("Silakan masukan Nama Lengkap.");
            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Silakan masukan Nama Lengkap.");

            /* Email Testing */
            RuleFor(model => model.Email)
                .NotNull()
                .WithMessage("Silakan masukan Alamat Email.");
            RuleFor(model => model.Email)
                .NotEmpty()
                .WithMessage("Silakan masukan Alamat Email.");
            RuleFor(model => model.Email)
                .EmailAddress()
                .WithMessage("Alamat Email tidak valid.");

            /* Username Testing */
            RuleFor(model => model.Username)
                .NotNull()
                .WithMessage("Silakan masukan Nama Pengguna.");
            RuleFor(model => model.Username)
                .NotEmpty()
                .WithMessage("Silakan masukan Nama Pengguna.");
            RuleFor(model => model.Username)
                .Matches(new Regex(@"^[a-zA-Z0-9_\-.]+$"))
                .WithMessage("Nama Pengguna mengandung karakter terlarang.");

            /* Password Testing */
            RuleFor(model => model.Password)
                .NotNull()
                .WithMessage("Silakan masukan Kata Sandi.");
            RuleFor(model => model.Password)
                .NotEmpty()
                .WithMessage("Silakan masukan Kata Sandi.");
            RuleFor(model => model.Password)
                .MinimumLength(8)
                .WithMessage("Kata Sandi minimum terdiri dari 8 karakter.");
            RuleFor(model => model.Password)
                .Matches(new Regex("[A-Z]"))
                .WithMessage("Kata Sandi wajib mengandung huruf kapital (A-Z).");
            RuleFor(model => model.Password)
                .Matches(new Regex("[a-z]"))
                .WithMessage("Kata Sandi wajib mengandung huruf non-kapital (a-z).");
            RuleFor(model => model.Password)
                .Matches(new Regex("[0-9]"))
                .WithMessage("Kata Sandi wajib mengandung angka (0-9).");
            
        }
    }
}
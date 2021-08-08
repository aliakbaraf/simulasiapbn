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
    public class ChangeAccountValidator : Validator<Account>
    {

        public ChangeAccountValidator()
        {
            /* Name Testing */
            RuleFor(model => model.Name)
                .NotNull()
                .WithMessage("Silakan masukan Nama Lengkap Anda.");
            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Silakan masukan Nama Lengkap Anda.");

            /* Email Testing */
            RuleFor(model => model.Email)
                .NotNull()
                .WithMessage("Silakan masukan Alamat Email Anda.");
            RuleFor(model => model.Email)
                .NotEmpty()
                .WithMessage("Silakan masukan Alamat Email Anda.");
            RuleFor(model => model.Email)
                .EmailAddress()
                .WithMessage("Alamat Email tidak valid.");

            /* Username Testing */
            RuleFor(model => model.Username)
                .NotNull()
                .WithMessage("Silakan masukan Nama Pengguna pilihan Anda.");
            RuleFor(model => model.Username)
                .NotEmpty()
                .WithMessage("Silakan masukan Nama Pengguna pilihan Anda.");
            RuleFor(model => model.Username)
                .Matches(new Regex(@"^[a-zA-Z0-9_\-.]+$"))
                .WithMessage("Nama Pengguna mengandung karakter terlarang.");
        }
        
    }
}
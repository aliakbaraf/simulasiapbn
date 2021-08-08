﻿/*
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
	public class EditAnalystValidator : Validator<Account>
	{
		public EditAnalystValidator()
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
		}
	}
}
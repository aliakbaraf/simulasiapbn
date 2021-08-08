﻿/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
namespace SimulasiAPBN.Core.Enumerators
{
	public enum SignInStatusCode
	{
		Success,
		UsernameMismatch,
		PasswordMismatch,
		AccountNotActivated,
		InsufficientAccessRole,
		BruteForceProtection,
		SystemFailure,
		UnknownFailure
	}
}
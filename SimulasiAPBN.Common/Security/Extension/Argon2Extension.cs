/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using SimulasiAPBN.Common.Security.Cryptography;

namespace SimulasiAPBN.Common.Security.Extension
{
	public static class Argon2Extension
	{
		public static string Hash(this string plain)
		{
			var argon2 = new Argon2();
			var hash = argon2.Hash(plain);
			return Convert.ToBase64String(hash);
		}

		public static bool Validate(this string base64Hash, string plain)
		{
			var argon2 = new Argon2();
			var hash = Convert.FromBase64String(base64Hash);
			return argon2.Verify(plain, hash);
		}
	}
}
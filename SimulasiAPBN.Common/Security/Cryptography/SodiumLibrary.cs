/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Runtime.InteropServices;

namespace SimulasiAPBN.Common.Security.Cryptography
{
	public static class SodiumLibrary
	{
		private const string Name = "libsodium";
		// ReSharper disable once InconsistentNaming
		public const int crypto_pwhash_argon2id_ALG_ARGON2ID13 = 2;
		// ReSharper disable once InconsistentNaming
		public const long crypto_pwhash_argon2id_OPSLIMIT_SENSITIVE = 4;
		// ReSharper disable once InconsistentNaming
		public const int crypto_pwhash_argon2id_MEMLIMIT_SENSITIVE = 536870912;

		static SodiumLibrary()
		{
			sodium_init();
		}

		[DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void sodium_init();
        
		[DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void randombytes_buf(byte[] buffer, int size);

		[DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int crypto_pwhash(byte[] buffer, long bufferLen, byte[] password, long passwordLen, byte[] salt, long opsLimit, int memLimit, int alg);

	}
}
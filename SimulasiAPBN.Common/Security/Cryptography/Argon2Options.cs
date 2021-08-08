/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Text;

namespace SimulasiAPBN.Common.Security.Cryptography
{
	public class Argon2Options
	{
		public int Algorithm { get; } = SodiumLibrary.crypto_pwhash_argon2id_ALG_ARGON2ID13;
		public int BufferLength { get; set; } = 16;
		public Encoding Encoding { get; set; } = Encoding.UTF8;
		public long IterationLimit { get; set; } = SodiumLibrary.crypto_pwhash_argon2id_OPSLIMIT_SENSITIVE;
		public int MemoryLimit { get; set; } = SodiumLibrary.crypto_pwhash_argon2id_MEMLIMIT_SENSITIVE;
		public int SaltLength { get; set; } = 16;
	}
}
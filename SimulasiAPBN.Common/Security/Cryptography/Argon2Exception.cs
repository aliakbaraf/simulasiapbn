/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;

namespace SimulasiAPBN.Common.Security.Cryptography
{
	public class Argon2Exception : Exception
	{
		public Argon2Exception() {}
        
		public Argon2Exception(string message) : base(message) {}
	}
}
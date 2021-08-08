/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Linq;

namespace SimulasiAPBN.Common.Security.Cryptography
{
	public class Argon2
    {
        public Argon2Options Options { get; set; }

        public Argon2()
        {
            Options = new Argon2Options();
        }

        public Argon2(Argon2Options options)
        {
            Options = options;
        }
        
        public byte[] GenerateSalt()
        {
            var buffer = new byte[Options.SaltLength];
            SodiumLibrary.randombytes_buf(buffer, buffer.Length);
            return buffer;
        }

        public byte[] Hash(string password)
        {
            var salt = GenerateSalt();
            return Hash(password, salt);
        }
        
        public byte[] Hash(string password, byte[] salt)
        {
            var buffer = new byte[Options.BufferLength];

            var result = SodiumLibrary.crypto_pwhash(
                buffer,
                buffer.Length,
                Options.Encoding.GetBytes(password),
                password.Length,
                salt,
                Options.IterationLimit,
                Options.MemoryLimit,
                Options.Algorithm
            );

            if (result != 0)
            {
                throw new Argon2Exception($"An unexpected error has occurred. Error code: {result}");
            }
            
            var hash = new byte[buffer.Length + salt.Length];
            Buffer.BlockCopy(buffer, 0, hash, 0, buffer.Length);
            Buffer.BlockCopy(salt, 0, hash, buffer.Length, salt.Length);
            
            return hash;
        }

        public bool Verify(string password, byte[] hash)
        {
            var salt = new byte[Options.SaltLength];
            Buffer.BlockCopy(hash, Options.BufferLength, salt, 0, salt.Length);
            return Verify(password, hash, salt);
        } 

        public bool Verify(string password, byte[] hash, byte[] salt)
        {
            var againstHash = Hash(password, salt);
            return hash.SequenceEqual(againstHash);
        }
    }
}
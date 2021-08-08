/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using SimulasiAPBN.Common.Security.Extension;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Core.Extensions
{
    public static class AccountModelExtension
    {
        public static Account HashPassword(this Account account)
        {
            if (string.IsNullOrEmpty(account.Password))
            {
                return account;
            }

            account.Password = account.Password.Hash();   
            return account;
        }
        
        public static Account HashPassword(this Account account, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return account;
            }

            account.Password = password.Hash();
            return account;
        }

        public static bool ValidatePassword(this Account account, string password)
        {
            if (string.IsNullOrEmpty(account.Password) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            return account.Password.Validate(password);
        }
        
        public static IEnumerable<Claim> ToClaims(this Account account, Guid sessionId)
        {
            return new[]
            {
                new Claim(ClaimTypes.SerialNumber, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Name),
                new Claim(ClaimTypes.NameIdentifier, account.Username),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(AccountRole), account.Role) ?? string.Empty),
                new Claim(ClaimTypes.Sid, sessionId.ToString())
            };
        }
        
        public static Account ToAccount(this IEnumerable<Claim> claims)
        {
            var claimList = claims.ToList();
            return new Account
            {
                Id = new Guid(claimList.Find(claim => claim.Type == ClaimTypes.SerialNumber)
                    ?.Value ?? string.Empty),
                Name = claimList.Find(claim => claim.Type == ClaimTypes.Name)
                    ?.Value  ?? string.Empty,
                Username = claimList.Find(claim => claim.Type == ClaimTypes.NameIdentifier)
                    ?.Value  ?? string.Empty,
                Email = claimList.Find(claim => claim.Type == ClaimTypes.Email)
                    ?.Value ?? string.Empty,
                Role = (AccountRole) Enum.Parse(
                    typeof(AccountRole), 
                    claimList.Find(claim => claim.Type == ClaimTypes.Role)?.Value ?? string.Empty),
            };
        }
    }
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Extensions;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Infrastructure.Dapper.Repositories
{
    public class SignInAttemptRepository 
        : GenericRepository<SignInAttempt>, ISignInAttemptRepository<RepositoryOptions>
    {
        private static readonly TimeSpan SignInTimeThreshold = TimeSpan.FromMinutes(60);
        private const int MaxConsecutiveFailedAttempts = 5;
        
        public SignInAttemptRepository(RepositoryOptions options) : base(options)
        {
        }

        private static bool LastAnHourAgo(GenericModel entity)
        {
            var anHourAgo = DateTimeOffset.Now - SignInTimeThreshold;
            return entity.CreatedAt >= anHourAgo;
        } 
        
        public async Task<bool> IsCurrentlyBlockedAsync(Account account, string userAgent, IPAddress ipAddress)
        {
            if (account is null) return false;

            var entities = (await FindAsync(signInAttempt => signInAttempt.AccountId == account.Id 
                                                             && signInAttempt.IpAddress == ipAddress.ToString()))
                .Where(LastAnHourAgo)
                .OrderByDescending(entity => entity.CreatedAt)
                .ToList();
            
            if (entities.Count < MaxConsecutiveFailedAttempts) return false;
            
            var consecutiveFailedAttempts = 0;
            foreach (var entity in entities)
            {
                if (entity.IsSuccess) return false;
                
                consecutiveFailedAttempts++;
                if (consecutiveFailedAttempts < MaxConsecutiveFailedAttempts) continue;
                
                var signInAttempt = new SignInAttempt
                {
                    AccountId = account.Id,
                    UserAgent = userAgent,
                    IpAddress = ipAddress.ToString(),
                    IsSuccess = false,
                    StatusCode = SignInStatusCode.BruteForceProtection
                };
                await AddAsync(signInAttempt);
                
                return true;
            }
            return false;
        }

        public SignInAttempt SignIn(Account account, string password, string userAgent, IPAddress ipAddress)
        {
            var signInAttempt = new SignInAttempt
            {
                AccountId = account?.Id ?? Guid.Empty,
                UserAgent = userAgent,
                IpAddress = ipAddress.ToString(),
                IsSuccess = false,
                StatusCode = SignInStatusCode.UsernameMismatch
            };
            if (account is null || account.Id == Guid.Empty)
            {
                return signInAttempt;
            }

            signInAttempt.IsSuccess = account.ValidatePassword(password);
            signInAttempt.StatusCode = signInAttempt.IsSuccess
                ? SignInStatusCode.Success
                : SignInStatusCode.PasswordMismatch;
            Add(signInAttempt);
            return signInAttempt;
        }
    }
}
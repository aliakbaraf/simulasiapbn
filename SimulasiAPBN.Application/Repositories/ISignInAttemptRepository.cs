/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Net;
using System.Threading.Tasks;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface ISignInAttemptRepository : ISignInAttemptRepository<IRepositoryOptions> {}
    
    public interface ISignInAttemptRepository<out TRepositoryOptions> 
        : IGenericRepository<SignInAttempt, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        Task<bool> IsCurrentlyBlockedAsync(Account? account, string userAgent, IPAddress ipAddress);

        SignInAttempt? SignIn(Account? account, string password, string userAgent, IPAddress ipAddress);
    }
}
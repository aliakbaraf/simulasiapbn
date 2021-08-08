/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Threading.Tasks;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface ITokenRepository : ITokenRepository<IRepositoryOptions> {}
    
    public interface ITokenRepository<out TRepositoryOptions> : IGenericRepository<Token, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        Token? FindMatch(Account associatedAccount, string code, TokenType type);
        
        Task<Token?> FindMatchAsync(Account associatedAccount, string code, TokenType type);
    }
}
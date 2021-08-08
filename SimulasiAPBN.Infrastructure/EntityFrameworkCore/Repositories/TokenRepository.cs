/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Threading.Tasks;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Infrastructure.EntityFrameworkCore.Repositories
{
    public class TokenRepository : GenericRepository<Token>, ITokenRepository<RepositoryOptions>
    {
        public TokenRepository(RepositoryOptions options) : base(options) {}

        public Token FindMatch(Account account, string code, TokenType type)
        {
            return FindOne(
                token => token.AccountId == account.Id &&
                         token.Code == code &&
                         token.Type == type);
        }

        public Task<Token> FindMatchAsync(Account account, string code, TokenType type)
        {
            return FindOneAsync(
                token => token.AccountId == account.Id &&
                         token.Code == code &&
                         token.Type == type);
        }
    }
}
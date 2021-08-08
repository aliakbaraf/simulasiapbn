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

namespace SimulasiAPBN.Infrastructure.Dapper.Repositories
{
    public class WebContentRepository : GenericRepository<WebContent>, IWebContentRepository<RepositoryOptions>
    {
        public WebContentRepository(RepositoryOptions options) : base(options) {}

        public WebContent GetByKey(WebContentKey key)
        {
            return FindOne(webContent => webContent.Key == key);
        }

        public Task<WebContent> GetByKeyAsync(WebContentKey key)
        {
            return FindOneAsync(webContent => webContent.Key == key);
        }
    }
}
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
    public interface IWebContentRepository : IWebContentRepository<IRepositoryOptions> {}
    
    public interface IWebContentRepository<out TRepositoryOptions> : IGenericRepository<WebContent, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        WebContent? GetByKey(WebContentKey key);
        
        Task<WebContent?> GetByKeyAsync(WebContentKey key);
    }
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface IMediaFileRepository : IMediaFileRepository<IRepositoryOptions> {}
    
    public interface IMediaFileRepository<out TRepositoryOptions> : IGenericRepository<MediaFile, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions {}
}
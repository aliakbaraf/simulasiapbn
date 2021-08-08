/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface IAllocationRepository : IAllocationRepository<IRepositoryOptions> {}
    
    public interface IAllocationRepository<out TRepositoryOptions> 
        : IGenericRepository<Allocation, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions {}
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface IBudgetRepository : IBudgetRepository<IRepositoryOptions> {}
    
    public interface IBudgetRepository<out TRepositoryOptions>  : IGenericRepository<Budget, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions {}
}
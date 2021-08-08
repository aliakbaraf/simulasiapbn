/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface IBudgetTargetRepository : IBudgetTargetRepository<IRepositoryOptions> {}
    
    public interface IBudgetTargetRepository<out TRepositoryOptions> : IGenericRepository<BudgetTarget, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions {}
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Collections.Generic;
using System.Threading.Tasks;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface IStateExpenditureRepository : IStateExpenditureRepository<IRepositoryOptions> {}
    
    public interface IStateExpenditureRepository<out TRepositoryOptions> : IGenericRepository<StateExpenditure, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        IEnumerable<StateExpenditure> GetByStateBudget(StateBudget stateBudget);

        Task<IEnumerable<StateExpenditure>> GetByStateBudgetAsync(StateBudget stateBudget);
    }
}
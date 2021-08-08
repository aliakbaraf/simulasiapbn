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
    public interface IStateBudgetRepository : IStateBudgetRepository<IRepositoryOptions> {}
    
    public interface IStateBudgetRepository<out TRepositoryOptions> : IGenericRepository<StateBudget, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        IEnumerable<StateBudget> GetByYear(int year);
        
        Task<IEnumerable<StateBudget>> GetByYearAsync(int year);


        public StateBudget? GetActive();
        Task<StateBudget?> GetActiveAsync();
        StateBudget? GetActiveFromList(IEnumerable<StateBudget> stateBudgets);

        public StateBudget? GetActive(int year);
        Task<StateBudget?> GetActiveAsync(int year);
        StateBudget? GetActiveFromList(IEnumerable<StateBudget> stateBudgets, int year);

        StateBudget? GetLatest();
        Task<StateBudget?> GetLatestAsync();
        StateBudget? GetLatestFromList(IEnumerable<StateBudget> stateBudgets);

    }
}
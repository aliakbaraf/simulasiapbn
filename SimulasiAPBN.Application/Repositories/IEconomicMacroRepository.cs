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
    public interface IEconomicMacroRepository : IEconomicMacroRepository<IRepositoryOptions> {}
    
    public interface IEconomicMacroRepository<out TRepositoryOptions> 
        : IGenericRepository<EconomicMacro, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        IEnumerable<EconomicMacro> GetByStateBudget(StateBudget stateBudget);
        Task<IEnumerable<EconomicMacro>> GetByStateBudgetAsync(StateBudget stateBudget);
    }
}
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
    public interface ISpecialPolicyRepository : ISpecialPolicyRepository<IRepositoryOptions> {}
    
    public interface ISpecialPolicyRepository<out TRepositoryOptions> 
        : IGenericRepository<SpecialPolicy, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        IEnumerable<SpecialPolicy> GetByStateBudget(StateBudget stateBudget);
        Task<IEnumerable<SpecialPolicy>> GetByStateBudgetAsync(StateBudget stateBudget);
    }
}
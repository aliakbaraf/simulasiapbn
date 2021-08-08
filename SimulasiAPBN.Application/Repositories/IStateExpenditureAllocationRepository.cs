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
    public interface IStateExpenditureAllocationRepository
        : IStateExpenditureRepository<IRepositoryOptions> {}

    public interface IStateExpenditureAllocationRepository<out TRepositoryOptions>
        : IGenericRepository<StateExpenditureAllocation, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        IEnumerable<StateExpenditureAllocation> GetByAllocation(Allocation allocation);
        
        Task<IEnumerable<StateExpenditureAllocation>> GetByAllocationAsync(Allocation allocation);
        
        IEnumerable<StateExpenditureAllocation> GetByStateExpenditure(StateExpenditure stateExpenditure);
        
        Task<IEnumerable<StateExpenditureAllocation>> GetByStateExpenditureAsync(
            StateExpenditure stateExpenditure);
    }
}
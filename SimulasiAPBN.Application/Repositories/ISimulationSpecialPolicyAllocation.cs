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
    public interface ISimulationSpecialPolicyAllocationRepository 
        : ISimulationSpecialPolicyAllocationRepository<IRepositoryOptions> {}

    public interface ISimulationSpecialPolicyAllocationRepository<out TRepositoryOptions>
        : IGenericRepository<SimulationSpecialPolicyAllocation, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        IEnumerable<SimulationSpecialPolicyAllocation> GetBySimulationSession(SimulationSession simulationSession);
        
        Task<IEnumerable<SimulationSpecialPolicyAllocation>> GetBySimulationSessionAsync(SimulationSession simulationSession);
    }
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Collections.Generic;
using System.Threading.Tasks;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface ISimulationShareRepository : ISimulationShareRepository<IRepositoryOptions> {}

    public interface ISimulationShareRepository<out TRepositoryOptions>
        : IGenericRepository<SimulationShare, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        IEnumerable<SimulationShare> GetBySimulationSession(SimulationSession simulationSession);
        
        IEnumerable<SimulationShare> GetBySimulationSession(
            SimulationSession simulationSession,
            SimulationShareTarget simulationShareTarget);
        
        Task<IEnumerable<SimulationShare>> GetBySimulationSessionAsync(SimulationSession simulationSession);
        
        Task<IEnumerable<SimulationShare>> GetBySimulationSessionAsync(
            SimulationSession simulationSession,
            SimulationShareTarget simulationShareTarget);
    }
}
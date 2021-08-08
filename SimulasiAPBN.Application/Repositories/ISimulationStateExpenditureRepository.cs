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
    public interface ISimulationStateExpenditureRepository
        : ISimulationStateExpenditureRepository<IRepositoryOptions> {}

    public interface ISimulationStateExpenditureRepository<out TRepositoryOptions>
        : IGenericRepository<SimulationStateExpenditure, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        IEnumerable<SimulationStateExpenditure> GetBySimulationSession(SimulationSession simulationSession);
        Task<IEnumerable<SimulationStateExpenditure>> GetBySimulationSessionAsync(SimulationSession simulationSession);
    }
}
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
    public interface ISimulationEconomicMacroRepository
        : ISimulationEconomicMacroRepository<IRepositoryOptions> {}

    public interface ISimulationEconomicMacroRepository<out TRepositoryOptions>
        : IGenericRepository<SimulationEconomicMacro, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        IEnumerable<SimulationEconomicMacro> GetBySimulationSession(SimulationSession simulationSession);
        Task<IEnumerable<SimulationEconomicMacro>> GetBySimulationSessionAsync(SimulationSession simulationSession);
    }
}
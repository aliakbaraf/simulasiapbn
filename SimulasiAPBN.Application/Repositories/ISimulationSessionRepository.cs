/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface ISimulationSessionRepository : ISimulationSessionRepository<IRepositoryOptions> {}
    
    public interface ISimulationSessionRepository<out TRepositoryOptions> 
        : IGenericRepository<SimulationSession, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions {}
}
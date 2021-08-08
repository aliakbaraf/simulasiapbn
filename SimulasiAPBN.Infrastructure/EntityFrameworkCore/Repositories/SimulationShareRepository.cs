/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Infrastructure.EntityFrameworkCore.Repositories
{
    public class SimulationShareRepository
        : GenericRepository<SimulationShare>, ISimulationShareRepository<RepositoryOptions>
    {
        public SimulationShareRepository(RepositoryOptions options) : base(options) {}
        
        protected override IQueryable<SimulationShare> EntityQuery => base.EntityQuery
            .Include(e => e.SimulationSession);
        
        public IEnumerable<SimulationShare> GetBySimulationSession(SimulationSession simulationSession)
        {
            return Find(share => share.SimulationSessionId == simulationSession.Id);
        }

        public IEnumerable<SimulationShare> GetBySimulationSession(
            SimulationSession simulationSession, SimulationShareTarget simulationShareTarget)
        {
            return Find(share => share.SimulationSessionId == simulationSession.Id
                                 && share.Target == simulationShareTarget);
        }

        public Task<IEnumerable<SimulationShare>> GetBySimulationSessionAsync(
            SimulationSession simulationSession)
        {
            return FindAsync(share => share.SimulationSessionId == simulationSession.Id);
        }

        public Task<IEnumerable<SimulationShare>> GetBySimulationSessionAsync(
            SimulationSession simulationSession, SimulationShareTarget simulationShareTarget)
        {
            return FindAsync(share => share.SimulationSessionId == simulationSession.Id
                                      && share.Target == simulationShareTarget);
        }
    }
}
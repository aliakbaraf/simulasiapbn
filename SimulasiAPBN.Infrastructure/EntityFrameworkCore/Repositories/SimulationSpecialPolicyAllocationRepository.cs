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
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Infrastructure.EntityFrameworkCore.Repositories
{
    public class SimulationSpecialPolicyAllocationRepository : 
        GenericRepository<SimulationSpecialPolicyAllocation>, 
        ISimulationSpecialPolicyAllocationRepository<RepositoryOptions>
    {
        public SimulationSpecialPolicyAllocationRepository(RepositoryOptions options) : base(options) {}
        
        protected override IQueryable<SimulationSpecialPolicyAllocation> EntityQuery => base.EntityQuery
            .Include(e => e.SpecialPolicyAllocation);
        
        public IEnumerable<SimulationSpecialPolicyAllocation> GetBySimulationSession(SimulationSession simulationSession)
        {
            return Find(policyAllocation => policyAllocation.SimulationSessionId == simulationSession.Id);
        }

        public Task<IEnumerable<SimulationSpecialPolicyAllocation>> GetBySimulationSessionAsync(SimulationSession simulationSession)
        {
            return FindAsync(policyAllocation => policyAllocation.SimulationSessionId == simulationSession.Id);
        }
    }
}
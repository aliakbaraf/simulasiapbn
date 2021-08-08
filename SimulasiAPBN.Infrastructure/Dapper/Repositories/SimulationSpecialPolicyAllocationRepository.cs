/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions;

namespace SimulasiAPBN.Infrastructure.Dapper.Repositories
{
    public class SimulationSpecialPolicyAllocationRepository : 
        GenericRepository<SimulationSpecialPolicyAllocation>, 
        ISimulationSpecialPolicyAllocationRepository<RepositoryOptions>
    {
        public SimulationSpecialPolicyAllocationRepository(RepositoryOptions options) : base(options) {
            UseMapping(Mapper);
        }
        
        private static readonly Func<Func<
            SimulationSpecialPolicyAllocation, 
            SpecialPolicyAllocation, 
            SimulationSpecialPolicyAllocation
        >> Mapper = () =>
        {
            var dictionary = new Dictionary<Guid, SimulationSpecialPolicyAllocation>();

            return (simulationSpecialPolicyAllocationEntity, specialPolicyAllocationEntity) =>
            {
                if (!dictionary.TryGetValue(simulationSpecialPolicyAllocationEntity.Id,
                    out var simulationSpecialPolicyAllocation))
                {
                    simulationSpecialPolicyAllocation = simulationSpecialPolicyAllocationEntity;
                    dictionary.Add(simulationSpecialPolicyAllocation.Id, simulationSpecialPolicyAllocation);
                }
                
                if (specialPolicyAllocationEntity is null)
                {
                    return simulationSpecialPolicyAllocation;
                }

                simulationSpecialPolicyAllocation.SpecialPolicyAllocation = specialPolicyAllocationEntity;

                return simulationSpecialPolicyAllocation;
            };
        };

        protected override IExecutableSelectQuery<SimulationSpecialPolicyAllocation> SelectQueryProcessor(
            IExecutableSelectQuery<SimulationSpecialPolicyAllocation> query)
        {
            query = query.Join<SpecialPolicyAllocation>("SpecialPolicyAllocationId", "Id");
            return base.SelectQueryProcessor(query);
        }
        
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
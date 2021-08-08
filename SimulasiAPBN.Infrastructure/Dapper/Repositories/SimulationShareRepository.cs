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
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions;

namespace SimulasiAPBN.Infrastructure.Dapper.Repositories
{
    public class SimulationShareRepository
        : GenericRepository<SimulationShare>, ISimulationShareRepository<RepositoryOptions>
    {
        public SimulationShareRepository(RepositoryOptions options) : base(options)
        {
            UseMapping(Mapper);
        }
        
        private static readonly Func<Func<SimulationShare, SimulationSession, SimulationShare>> Mapper = () =>
        {
            var dictionary = new Dictionary<Guid, SimulationShare>();

            return (entity, simulationSession) =>
            {
                if (!dictionary.TryGetValue(entity.Id, out var simulationShare))
                {
                    simulationShare = entity;
                    dictionary.Add(simulationShare.Id, simulationShare);
                }

                if (simulationSession is not null)
                {
                    simulationShare.SimulationSession = simulationSession;
                }
                return simulationShare;
            };
        };

        protected override IExecutableSelectQuery<SimulationShare> SelectQueryProcessor(
            IExecutableSelectQuery<SimulationShare> query)
        {
            query = query.Join<SimulationSession>("SimulationSessionId", "Id");
            return base.SelectQueryProcessor(query);
        }

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
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions;

namespace SimulasiAPBN.Infrastructure.Dapper.Repositories
{
    public class SimulationSessionRepository 
        : GenericRepository<SimulationSession>, ISimulationSessionRepository<RepositoryOptions>
    {
        public SimulationSessionRepository(RepositoryOptions options) : base(options)
        {
            UseMapping(Mapper);
        }
        
        private static readonly Func<Func<SimulationSession, StateBudget, SimulationSession>> Mapper = () =>
        {
            var dictionary = new Dictionary<Guid, SimulationSession>();

            return (entity, stateBudget) =>
            {
                if (!dictionary.TryGetValue(entity.Id, out var simulationSession))
                {
                    simulationSession = entity;
                    dictionary.Add(simulationSession.Id, simulationSession);
                }

                if (stateBudget is not null)
                {
                    simulationSession.StateBudget = stateBudget;
                }
                return simulationSession;
            };
        };

        protected override IExecutableSelectQuery<SimulationSession> SelectQueryProcessor(
            IExecutableSelectQuery<SimulationSession> query)
        {
            query = query.Join<StateBudget>("StateBudgetId", "Id");
            return base.SelectQueryProcessor(query);
        }
    }
}
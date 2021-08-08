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
    public class SimulationStateExpenditureRepository 
        : GenericRepository<SimulationStateExpenditure>, ISimulationStateExpenditureRepository<RepositoryOptions>
    {
        public SimulationStateExpenditureRepository(RepositoryOptions options) : base(options)
        {
            UseMapping(Mapper);
        }
        
        private static readonly Func<Func<SimulationStateExpenditure, StateExpenditure, SimulationStateExpenditure>> Mapper = () =>
        {
            var dictionary = new Dictionary<Guid, SimulationStateExpenditure>();

            return (simulationStateExpenditureEntity, stateExpenditureEntity) =>
            {
                if (!dictionary.TryGetValue(simulationStateExpenditureEntity.Id,
                    out var simulationStateExpenditures))
                {
                    simulationStateExpenditures = simulationStateExpenditureEntity;
                    dictionary.Add(simulationStateExpenditures.Id, simulationStateExpenditures);
                }
                
                if (stateExpenditureEntity is null)
                {
                    return simulationStateExpenditureEntity;
                }

                simulationStateExpenditures.StateExpenditure = stateExpenditureEntity;

                return simulationStateExpenditures;
            };
        };

        protected override IExecutableSelectQuery<SimulationStateExpenditure> SelectQueryProcessor(
            IExecutableSelectQuery<SimulationStateExpenditure> query)
        {
            query = query.Join<StateExpenditure>("StateExpenditureId", "Id");
            return base.SelectQueryProcessor(query);
        }
        
        public IEnumerable<SimulationStateExpenditure> GetBySimulationSession(SimulationSession simulationSession)
        {
            return Find(expenditure => expenditure.SimulationSessionId == simulationSession.Id);
        }

        public Task<IEnumerable<SimulationStateExpenditure>> GetBySimulationSessionAsync(SimulationSession simulationSession)
        {
            return FindAsync(expenditure => expenditure.SimulationSessionId == simulationSession.Id);
        }
    }
}
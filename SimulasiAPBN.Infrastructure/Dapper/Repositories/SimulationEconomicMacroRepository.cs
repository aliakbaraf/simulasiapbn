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
    public class SimulationEconomicMacroRepository 
        : GenericRepository<SimulationEconomicMacro>, ISimulationEconomicMacroRepository<RepositoryOptions>
    {
        public SimulationEconomicMacroRepository(RepositoryOptions options) : base(options)
        {
            UseMapping(Mapper);
        }
        
        private static readonly Func<Func<SimulationEconomicMacro, EconomicMacro, SimulationEconomicMacro>> Mapper = () =>
        {
            var dictionary = new Dictionary<Guid, SimulationEconomicMacro>();

            return (simulationEconomicMacroEntity, EconomicMacroEntity) =>
            {
                if (!dictionary.TryGetValue(simulationEconomicMacroEntity.Id,
                    out var simulationEconomicMacros))
                {
                    simulationEconomicMacros = simulationEconomicMacroEntity;
                    dictionary.Add(simulationEconomicMacros.Id, simulationEconomicMacros);
                }
                
                if (EconomicMacroEntity is null)
                {
                    return simulationEconomicMacroEntity;
                }

                simulationEconomicMacros.EconomicMacro = EconomicMacroEntity;

                return simulationEconomicMacros;
            };
        };

        protected override IExecutableSelectQuery<SimulationEconomicMacro> SelectQueryProcessor(
            IExecutableSelectQuery<SimulationEconomicMacro> query)
        {
            query = query.Join<EconomicMacro>("EconomicMacrosId", "Id");
            return base.SelectQueryProcessor(query);
        }
        
        public IEnumerable<SimulationEconomicMacro> GetBySimulationSession(SimulationSession simulationSession)
        {
            return Find(economicMacro => economicMacro.SimulationSessionId == simulationSession.Id);
        }

        public Task<IEnumerable<SimulationEconomicMacro>> GetBySimulationSessionAsync(SimulationSession simulationSession)
        {
            return FindAsync(economicMacro => economicMacro.SimulationSessionId == simulationSession.Id);
        }
    }
}
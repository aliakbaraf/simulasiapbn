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
    public class SimulationEconomicMacroRepository :
        GenericRepository<SimulationEconomicMacro>, 
        ISimulationEconomicMacroRepository<RepositoryOptions>
    {
        public SimulationEconomicMacroRepository(RepositoryOptions options) : base(options)
        {}
        
        protected override IQueryable<SimulationEconomicMacro> EntityQuery => base.EntityQuery
            .Include(e => e.EconomicMacro);

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
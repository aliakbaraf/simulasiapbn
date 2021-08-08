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
    public class SimulationStateExpenditureRepository :
        GenericRepository<SimulationStateExpenditure>, 
        ISimulationStateExpenditureRepository<RepositoryOptions>
    {
        public SimulationStateExpenditureRepository(RepositoryOptions options) : base(options)
        {}
        
        protected override IQueryable<SimulationStateExpenditure> EntityQuery => base.EntityQuery
            .Include(e => e.StateExpenditure);

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
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Infrastructure.EntityFrameworkCore.Repositories
{
    public class SimulationSessionRepository 
        : GenericRepository<SimulationSession>, ISimulationSessionRepository<RepositoryOptions>
    {
        public SimulationSessionRepository(RepositoryOptions options) : base(options) {}
        
        protected override IQueryable<SimulationSession> EntityQuery => base.EntityQuery
            .Include(e => e.StateBudget);
    }
}
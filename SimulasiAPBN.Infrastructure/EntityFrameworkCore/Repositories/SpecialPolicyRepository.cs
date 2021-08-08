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
    public class SpecialPolicyRepository 
        : GenericRepository<SpecialPolicy>, ISpecialPolicyRepository<RepositoryOptions>
    {
        public SpecialPolicyRepository(RepositoryOptions options) : base(options) {}

        protected override IQueryable<SpecialPolicy> EntityQuery => base.EntityQuery
            .Include(e => e.SpecialPolicyAllocations)
            .ThenInclude(e => e.Allocation);

        public IEnumerable<SpecialPolicy> GetByStateBudget(StateBudget stateBudget)
        {
            return Find(policy => policy.StateBudgetId == stateBudget.Id);
        }

        public Task<IEnumerable<SpecialPolicy>> GetByStateBudgetAsync(StateBudget stateBudget)
        {
            return FindAsync(policy => policy.StateBudgetId == stateBudget.Id);
        }
    }
}
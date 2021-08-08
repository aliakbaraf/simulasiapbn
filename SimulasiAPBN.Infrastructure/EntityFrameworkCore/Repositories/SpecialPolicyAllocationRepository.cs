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
    public class SpecialPolicyAllocationRepository : 
        GenericRepository<SpecialPolicyAllocation>,
        ISpecialPolicyAllocationRepository<RepositoryOptions>
    {
        public SpecialPolicyAllocationRepository(RepositoryOptions options) : base(options) {}
        
        protected override IQueryable<SpecialPolicyAllocation> EntityQuery => base.EntityQuery
            .Include(e => e.Allocation);

        public IEnumerable<SpecialPolicyAllocation> GetByAllocation(Allocation allocation)
        {
            return Find(policyAllocation => policyAllocation.AllocationId == allocation.Id);
        }

        public Task<IEnumerable<SpecialPolicyAllocation>> GetByAllocationAsync(Allocation allocation)
        {
            return FindAsync(policyAllocation => policyAllocation.AllocationId == allocation.Id);
        }

        public IEnumerable<SpecialPolicyAllocation> GetBySpecialPolicy(SpecialPolicy specialPolicy)
        {
            return Find(policyAllocation => policyAllocation.SpecialPolicyId == specialPolicy.Id);
        }

        public Task<IEnumerable<SpecialPolicyAllocation>> GetBySpecialPolicyAsync(SpecialPolicy specialPolicy)
        {
            return FindAsync(policyAllocation => policyAllocation.SpecialPolicyId == specialPolicy.Id);
        }
    }
}
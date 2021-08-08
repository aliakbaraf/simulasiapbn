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
    public class SpecialPolicyAllocationRepository 
        : GenericRepository<SpecialPolicyAllocation>, ISpecialPolicyAllocationRepository<RepositoryOptions>
    {
        public SpecialPolicyAllocationRepository(RepositoryOptions options) : base(options)
        {
            UseMapping(Mapper);
        }
        
        private static readonly Func<
            Func<SpecialPolicyAllocation, Allocation, SpecialPolicyAllocation>
        > Mapper = () =>
        { 
            return (specialPolicyAllocation, allocation) =>
            {
                specialPolicyAllocation.Allocation = allocation;
                return specialPolicyAllocation;
            };
        };
        
        protected override IExecutableSelectQuery<SpecialPolicyAllocation> SelectQueryProcessor(
            IExecutableSelectQuery<SpecialPolicyAllocation> query)
        {
            query = query.Join<Allocation>("AllocationId", "Id");
            return base.SelectQueryProcessor(query);
        }
        
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
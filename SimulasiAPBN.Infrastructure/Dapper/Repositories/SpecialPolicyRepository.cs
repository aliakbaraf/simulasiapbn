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
    public class SpecialPolicyRepository 
        : GenericRepository<SpecialPolicy>, ISpecialPolicyRepository<RepositoryOptions>
    {
        public SpecialPolicyRepository(RepositoryOptions options) : base(options)
        {
            UseMapping(Mapper);
        }

        private static readonly Func<Func<SpecialPolicy, SpecialPolicyAllocation, Allocation, SpecialPolicy>> Mapper = () =>
        {
            var dictionary = new Dictionary<Guid, SpecialPolicy>();

            return (entity, specialPolicyAllocation, allocation) =>
            {
                if (!dictionary.TryGetValue(entity.Id, out var specialPolicy))
                {
                    specialPolicy = entity;
                    specialPolicy.SpecialPolicyAllocations = new List<SpecialPolicyAllocation>();
                    dictionary.Add(specialPolicy.Id, specialPolicy);
                }

                if (specialPolicyAllocation is null) return specialPolicy;
                
                if (allocation is not null)
                {
                    specialPolicyAllocation.Allocation = allocation;
                }
                specialPolicy.SpecialPolicyAllocations.Add(specialPolicyAllocation);
                
                return specialPolicy;
            };
        };

        protected override IExecutableSelectQuery<SpecialPolicy> SelectQueryProcessor(
            IExecutableSelectQuery<SpecialPolicy> query)
        {
            query = query.Join<SpecialPolicyAllocation>("Id", "SpecialPolicyId")
                .ThenJoin<Allocation>("AllocationId", "Id");
            return base.SelectQueryProcessor(query);
        }

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
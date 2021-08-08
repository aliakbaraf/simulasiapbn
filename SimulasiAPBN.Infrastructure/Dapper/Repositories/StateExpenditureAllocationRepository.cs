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
    public class StateExpenditureAllocationRepository : 
        GenericRepository<StateExpenditureAllocation>, 
        IStateExpenditureAllocationRepository<RepositoryOptions>
    {
        public StateExpenditureAllocationRepository(RepositoryOptions options) : base(options) 
        {
            UseMapping(Mapper);
        }

        private static readonly Func<
            Func<StateExpenditureAllocation, Allocation, StateExpenditureAllocation>
        > Mapper = () =>
        { 
            return (stateExpenditureAllocation, allocation) =>
            {
                stateExpenditureAllocation.Allocation = allocation;
                return stateExpenditureAllocation;
            };
        };
        
        protected override IExecutableSelectQuery<StateExpenditureAllocation> SelectQueryProcessor(
            IExecutableSelectQuery<StateExpenditureAllocation> query)
        {
            query = query.Join<Allocation>("AllocationId", "Id");
            return base.SelectQueryProcessor(query);
        }

        public IEnumerable<StateExpenditureAllocation> GetByAllocation(Allocation allocation)
        {
            return Find(expenditureAllocation => 
                expenditureAllocation.AllocationId == allocation.Id);
        }

        public Task<IEnumerable<StateExpenditureAllocation>> GetByAllocationAsync(Allocation allocation)
        {
            return FindAsync(expenditureAllocation => 
                expenditureAllocation.AllocationId == allocation.Id);
        }

        public IEnumerable<StateExpenditureAllocation> GetByStateExpenditure(StateExpenditure stateExpenditure)
        {
            return Find(expenditureAllocation => 
                expenditureAllocation.StateExpenditureId == stateExpenditure.Id);
        }

        public Task<IEnumerable<StateExpenditureAllocation>> GetByStateExpenditureAsync(StateExpenditure stateExpenditure)
        {
            return FindAsync(expenditureAllocation => 
                expenditureAllocation.StateExpenditureId == stateExpenditure.Id);
        }
    }
}
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
    public class StateExpenditureAllocationRepository 
        : GenericRepository<StateExpenditureAllocation>, IStateExpenditureAllocationRepository<RepositoryOptions>
    {
        public StateExpenditureAllocationRepository(RepositoryOptions options) : base(options) {}
        
        protected override IQueryable<StateExpenditureAllocation> EntityQuery => base.EntityQuery
            .Include(e => e.Allocation);

        public IEnumerable<StateExpenditureAllocation> GetByAllocation(Allocation allocation)
        {
            return Find(expenditureAllocation => 
                expenditureAllocation.Allocation == allocation);
        }

        public Task<IEnumerable<StateExpenditureAllocation>> GetByAllocationAsync(Allocation allocation)
        {
            return FindAsync(expenditureAllocation => 
                expenditureAllocation.Allocation == allocation);
        }

        public IEnumerable<StateExpenditureAllocation> GetByStateExpenditure(StateExpenditure stateExpenditure)
        {
            return Find(expenditureAllocation => 
                expenditureAllocation.StateExpenditure == stateExpenditure);
        }

        public Task<IEnumerable<StateExpenditureAllocation>> GetByStateExpenditureAsync(StateExpenditure stateExpenditure)
        {
            return FindAsync(expenditureAllocation => 
                expenditureAllocation.StateExpenditure == stateExpenditure);
        }
    }
}
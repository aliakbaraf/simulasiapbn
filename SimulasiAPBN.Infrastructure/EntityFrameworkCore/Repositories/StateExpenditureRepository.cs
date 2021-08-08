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
    public class StateExpenditureRepository 
        : GenericRepository<StateExpenditure>, IStateExpenditureRepository<RepositoryOptions>
    {
        public StateExpenditureRepository(RepositoryOptions options) : base(options) {}

        protected override IQueryable<StateExpenditure> EntityQuery => base.EntityQuery
            .Include(e => e.Budget)
            .ThenInclude(e => e.BudgetTargets);

        public IEnumerable<StateExpenditure> GetByStateBudget(StateBudget stateBudget)
        {
            return Find(expenditure => expenditure.StateBudget == stateBudget);
        }

        public Task<IEnumerable<StateExpenditure>> GetByStateBudgetAsync(StateBudget stateBudget)
        {
            return FindAsync(expenditure => expenditure.StateBudget == stateBudget);
        }
    }
}
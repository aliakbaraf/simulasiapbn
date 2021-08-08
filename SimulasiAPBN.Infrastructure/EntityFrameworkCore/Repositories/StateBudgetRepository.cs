/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Infrastructure.EntityFrameworkCore.Repositories
{
    public class StateBudgetRepository : GenericRepository<StateBudget>, IStateBudgetRepository<RepositoryOptions>
    {
        public StateBudgetRepository(RepositoryOptions options) : base(options) {}

        protected override IQueryable<StateBudget> EntityQuery => base.EntityQuery
            .Include(e => e.SpecialPolicies)
            .Include(e => e.StateExpenditures)
                .ThenInclude(e => e.Budget);

        public IEnumerable<StateBudget> GetByYear(int year)
        {
            return Find(stateBudget => stateBudget.Year == year);
        }

        public Task<IEnumerable<StateBudget>> GetByYearAsync(int year)
        {
            return FindAsync(stateBudget => stateBudget.Year == year);
        }

        public StateBudget GetActive()
        {
            var stateBudgets = GetAll();
            return GetActiveFromList(stateBudgets);
        }

        public StateBudget GetActive(int year)
        {
            var stateBudgets = GetAll();
            return GetActiveFromList(stateBudgets, year);
        }

        public async Task<StateBudget> GetActiveAsync()
        {
            var stateBudgets = await GetAllAsync();
            return GetActiveFromList(stateBudgets);
        }

        public async Task<StateBudget> GetActiveAsync(int year)
        {
            var stateBudgets = await GetAllAsync();
            return GetActiveFromList(stateBudgets, year);
        }

        public StateBudget GetActiveFromList(IEnumerable<StateBudget> stateBudgets)
        {
            return GetActiveFromList(stateBudgets, DateTimeOffset.Now.Year);
        }

        public StateBudget GetActiveFromList(IEnumerable<StateBudget> stateBudgets, int year)
        {
            var filteredStateBudgets = stateBudgets
                .Where(e => e.Year == year && e.CountryIncome > 0 && e.StateExpenditures.Any())
                .OrderByDescending(e => e.Revision);

            return (
                    from stateBudget in filteredStateBudgets
                    let totalExpenditures = stateBudget.StateExpenditures
                        .Sum(e => e.TotalAllocation)
                    where totalExpenditures > 0
                    select stateBudget)
                .FirstOrDefault();
        }

        public StateBudget GetLatest()
        {
            var stateBudgets = GetAll();
            return GetLatestFromList(stateBudgets);
        }

        public async Task<StateBudget> GetLatestAsync()
        {
            var stateBudgets = await GetAllAsync();
            return GetLatestFromList(stateBudgets);
        }

        public StateBudget GetLatestFromList(IEnumerable<StateBudget> stateBudgets)
        {
            return stateBudgets
                .OrderByDescending(e => e.Year)
                    .ThenByDescending(e => e.Revision)
                .FirstOrDefault();
        }
        
    }
}
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
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions;

namespace SimulasiAPBN.Infrastructure.Dapper.Repositories
{
    public class StateBudgetRepository : GenericRepository<StateBudget>, IStateBudgetRepository<RepositoryOptions>
    {
        public StateBudgetRepository(RepositoryOptions options) : base(options)
        {
            UseMapping(Mapper);
        }
        
        private static readonly Func<Func<StateBudget, SpecialPolicy, StateExpenditure, Budget, StateBudget>> Mapper = () =>
        {
            var dictionary = new Dictionary<Guid, StateBudget>();

            return (entity, specialPolicy, stateExpenditure, budget) =>
            {
                if (!dictionary.TryGetValue(entity.Id, out var stateBudget))
                {
                    stateBudget = entity;
                    stateBudget.SpecialPolicies = new List<SpecialPolicy>();
                    stateBudget.StateExpenditures = new List<StateExpenditure>();
                    stateBudget.EconomicMacros = new List<EconomicMacro>();
                    dictionary.Add(stateBudget.Id, stateBudget);
                }

                if (specialPolicy is not null &&
                    stateBudget.SpecialPolicies.All(e => e.Id != specialPolicy.Id))
                {
                    stateBudget.SpecialPolicies.Add(specialPolicy);
                }

                if (stateExpenditure is null ||
                    stateBudget.StateExpenditures.Any(e => e.Id == stateExpenditure.Id))
                {
                    return stateBudget;
                }

                if (budget is not null)
                {
                    stateExpenditure.Budget = budget;
                }
                stateBudget.StateExpenditures.Add(stateExpenditure);
                
                return stateBudget;
            };
        };

        protected override IExecutableSelectQuery<StateBudget> SelectQueryProcessor(
            IExecutableSelectQuery<StateBudget> query)
        {
            query = query.Join<SpecialPolicy>("Id", "StateBudgetId")
                .Join<StateExpenditure>("Id", "StateBudgetId")
                .ThenJoin<Budget>("BudgetId", "Id");
            return base.SelectQueryProcessor(query);
        }

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
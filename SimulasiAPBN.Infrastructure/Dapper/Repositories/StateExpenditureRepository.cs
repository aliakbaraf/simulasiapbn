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
    public class StateExpenditureRepository 
        : GenericRepository<StateExpenditure>, IStateExpenditureRepository<RepositoryOptions>
    {
        public StateExpenditureRepository(RepositoryOptions options) : base(options)
        {
            UseMapping(Mapper);
        }
        
        private static readonly Func<Func<StateExpenditure, Budget, BudgetTarget, StateExpenditure>> Mapper = () =>
        {
            var stateExpenditureDictionary = new Dictionary<Guid, StateExpenditure>();
            var budgetDictionary = new Dictionary<Guid, Budget>();

            return (stateExpenditureEntity, budgetEntity, budgetTarget) =>
            {
                if (!stateExpenditureDictionary.TryGetValue(stateExpenditureEntity.Id, out var stateExpenditure))
                {
                    stateExpenditure = stateExpenditureEntity;
                    stateExpenditure.StateExpenditureAllocations = new List<StateExpenditureAllocation>();
                    stateExpenditureDictionary.Add(stateExpenditure.Id, stateExpenditure);
                }
                
                if (budgetEntity is null)
                {
                    return stateExpenditure;
                }
                
                if (!budgetDictionary.TryGetValue(budgetEntity.Id, out var budget))
                {
                    budget = budgetEntity;
                    budget.BudgetTargets = new List<BudgetTarget>();
                    budgetDictionary.Add(budget.Id, budget);
                }

                if (budgetTarget is not null)
                {
                    budget.BudgetTargets.Add(budgetTarget);
                }

                stateExpenditure.Budget = budget;

                return stateExpenditure;
            };
        };

        protected override IExecutableSelectQuery<StateExpenditure> SelectQueryProcessor(
            IExecutableSelectQuery<StateExpenditure> query)
        {
            query = query.Join<Budget>("BudgetId", "Id")
                .ThenJoin<BudgetTarget>("Id", "BudgetId");
            return base.SelectQueryProcessor(query);
        }

        public IEnumerable<StateExpenditure> GetByStateBudget(StateBudget stateBudget)
        {
            return Find(expenditure => expenditure.StateBudgetId == stateBudget.Id);
        }

        public Task<IEnumerable<StateExpenditure>> GetByStateBudgetAsync(StateBudget stateBudget)
        {
            return FindAsync(expenditure => expenditure.StateBudgetId == stateBudget.Id);
        }
    }
}
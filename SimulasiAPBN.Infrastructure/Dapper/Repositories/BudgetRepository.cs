/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions;

namespace SimulasiAPBN.Infrastructure.Dapper.Repositories
{
    public class BudgetRepository : GenericRepository<Budget>, IBudgetRepository<RepositoryOptions>
    {
        public BudgetRepository(RepositoryOptions options) : base(options)
        {
            UseMapping(Mapper);
        }

        private static readonly Func<Func<Budget, BudgetTarget, Budget>> Mapper = () =>
        {
            var dictionary = new Dictionary<Guid, Budget>();

            return (entity, budgetTarget) =>
            {
                if (!dictionary.TryGetValue(entity.Id, out var budget))
                {
                    budget = entity;
                    budget.BudgetTargets = new List<BudgetTarget>();
                    dictionary.Add(budget.Id, budget);
                }

                if (budgetTarget is not null)
                {
                    budget.BudgetTargets.Add(budgetTarget);
                }
                return budget;
            };
        };

        protected override IExecutableSelectQuery<Budget> SelectQueryProcessor(
            IExecutableSelectQuery<Budget> query)
        {
            query = query.Join<BudgetTarget>("Id", "BudgetId");
            return base.SelectQueryProcessor(query);
        }
    }
}
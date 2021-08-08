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
    public class EconomicMacroRepository : GenericRepository<EconomicMacro>, IEconomicMacroRepository<RepositoryOptions>
    {
        public EconomicMacroRepository(RepositoryOptions options) : base(options) {}

        public IEnumerable<EconomicMacro> GetByStateBudget(StateBudget stateBudget)
        {
            return Find(economicMacro => economicMacro.StateBudgetId == stateBudget.Id);
        }

        public Task<IEnumerable<EconomicMacro>> GetByStateBudgetAsync(StateBudget stateBudget)
        {
            return FindAsync(economicMacro => economicMacro.StateBudgetId == stateBudget.Id);
        }
    }
}
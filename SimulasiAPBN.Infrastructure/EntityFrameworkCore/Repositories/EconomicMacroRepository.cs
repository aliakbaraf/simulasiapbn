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
    public class EconomicMacroRepository : GenericRepository<EconomicMacro>,
        IEconomicMacroRepository<RepositoryOptions>
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
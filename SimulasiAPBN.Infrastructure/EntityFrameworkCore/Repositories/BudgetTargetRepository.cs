/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Infrastructure.EntityFrameworkCore.Repositories
{
    public class BudgetTargetRepository : GenericRepository<BudgetTarget>, IBudgetTargetRepository<RepositoryOptions>
    {
        public BudgetTargetRepository(RepositoryOptions options) : base(options) {}
    }
}
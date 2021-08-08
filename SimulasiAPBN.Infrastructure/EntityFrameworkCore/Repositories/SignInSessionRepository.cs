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
	public class SignInSessionRepository : GenericRepository<SignInSession>, ISignInSessionRepository<RepositoryOptions>
	{
		public SignInSessionRepository(RepositoryOptions options) : base(options) {}
		
		protected override IQueryable<SignInSession> EntityQuery => base.EntityQuery
			.Include(e => e.SignInAttempt);
		
		public IEnumerable<SignInSession> GetByAccount(Account account)
		{
			return Find(entity => entity.SignInAttempt.AccountId == account.Id);
		}

		public Task<IEnumerable<SignInSession>> GetByAccountAsync(Account account)
		{
			return FindAsync(entity => entity.SignInAttempt.AccountId == account.Id);
		}
	}
}
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
	public class SignInSessionRepository : GenericRepository<SignInSession>, ISignInSessionRepository<RepositoryOptions>
	{
		public SignInSessionRepository(RepositoryOptions options) : base(options)
		{
			UseMapping(Mapper);
		}
		
		private static readonly Func<Func<SignInSession, SignInAttempt, SignInSession>> Mapper = () =>
		{
			var dictionary = new Dictionary<Guid, SignInSession>();

			return (entity, signInAttempt) =>
			{
				if (dictionary.TryGetValue(entity.Id, out var signInSession))
				{
					return signInSession;
				}
				
				signInSession = entity;
				signInSession.SignInAttempt = signInAttempt;
				dictionary.Add(signInSession.Id, signInSession);
				
				return signInSession;
			};
		};
		
		protected override IExecutableSelectQuery<SignInSession> SelectQueryProcessor(
			IExecutableSelectQuery<SignInSession> query)
		{
			query = query.Join<SignInAttempt>("SignInAttemptId", "Id");
			return base.SelectQueryProcessor(query);
		}
		
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
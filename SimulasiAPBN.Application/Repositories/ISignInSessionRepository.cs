/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Collections.Generic;
using System.Threading.Tasks;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
	public interface ISignInSessionRepository : ISignInSessionRepository<IRepositoryOptions> {}
	
	public interface ISignInSessionRepository<out TRepositoryOptions> 
		: IGenericRepository<SignInSession, TRepositoryOptions>
		where TRepositoryOptions : IRepositoryOptions
	{
		IEnumerable<SignInSession> GetByAccount(Account account);

		Task<IEnumerable<SignInSession>> GetByAccountAsync(Account account);
	}
}
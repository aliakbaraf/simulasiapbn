/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Threading.Tasks;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface IAccountRepository : IAccountRepository<IRepositoryOptions> {}
    
    public interface IAccountRepository<out TRepositoryOptions> : IGenericRepository<Account, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        void Activate(Account entity);
        Task ActivateAsync(Account entity);
        void Deactivate(Account entity);
        Task DeactivateAsync(Account entity);
        
        Account? GetByUsername(string username);
        
        Task<Account?> GetByUsernameAsync(string username);

        bool IsPasswordCorrect(Account? entity);
        
        Task<bool> IsPasswordCorrectAsync(Account? entity);

        void ModifyPassword(Account? entity);
        
        void ModifyPassword(Account? entity, string newPassword);
        
        Task ModifyPasswordAsync(Account? entity);
        
        Task ModifyPasswordAsync(Account? entity, string newPassword);
    }
}
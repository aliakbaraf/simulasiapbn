/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Threading.Tasks;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Extensions;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Infrastructure.EntityFrameworkCore.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository<RepositoryOptions>
    {
        public AccountRepository(RepositoryOptions options) : base(options)
        {
        }

        public override int Add(Account entity)
        {
            entity = entity.HashPassword();
            
            return base.Add(entity);
        }

        public override Task<int> AddAsync(Account entity)
        {
            entity = entity.HashPassword();
            
            return base.AddAsync(entity);
        }

        public void Activate(Account entity)
        {
            entity.IsActivated = true;
            Modify(entity);
        }
        
        public Task ActivateAsync(Account entity)
        {
            entity.IsActivated = true;
            return ModifyAsync(entity);
        }

        public void Deactivate(Account entity)
        {
            if (entity.Role == AccountRole.DeveloperSupport)
            {
                return;
            }
            
            entity.IsActivated = false;
            Modify(entity);
        }
        
        public async Task DeactivateAsync(Account entity)
        {
            if (entity.Role == AccountRole.DeveloperSupport)
            {
                return;
            }
            
            entity.IsActivated = false;
            await ModifyAsync(entity);
        }

        public Account GetByUsername(string username)
        {
            return FindOne(account => account.Username == username);
        }

        public Task<Account> GetByUsernameAsync(string username)
        {
            return FindOneAsync(account => account.Username == username);
        }


        public bool IsPasswordCorrect(Account entity)
        {
            if (entity is null)
            {
                return false;
            }
            
            var account = GetByUsername(entity.Username);
            return account is not null && account.ValidatePassword(entity.Password);
        }

        public async Task<bool> IsPasswordCorrectAsync(Account entity)
        {
            if (entity is null)
            {
                return false;
            }
            
            var account = await GetByUsernameAsync(entity.Username);
            return account is not null && account.ValidatePassword(entity.Password);
        }

        public void ModifyPassword(Account entity)
        {
            entity = entity.HashPassword();
            Modify(entity);
        }

        public void ModifyPassword(Account entity, string newPassword)
        {
            entity = entity.HashPassword(newPassword);
            Modify(entity);
        }

        public Task ModifyPasswordAsync(Account entity)
        {
            entity = entity.HashPassword();
            return ModifyAsync(entity);
        }

        public Task ModifyPasswordAsync(Account entity, string newPassword)
        {
            entity = entity.HashPassword(newPassword);
            return ModifyAsync(entity);
        }
    }
}
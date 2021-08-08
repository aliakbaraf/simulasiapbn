/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Common.Configuration;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Extensions;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Validation;
using AccountEntity = SimulasiAPBN.Core.Models.Account;

namespace SimulasiAPBN.Web.Pages.Dashboard.Account
{
    public class Index : BasePage
    {
        private readonly IValidatorFactory _validatorFactory;

        public Index(
            IConfiguration configuration,
            IMemoryCache memoryCache, 
            IUnitOfWork unitOfWork, 
            IValidatorFactory validatorFactory) 
            : base(configuration, memoryCache, unitOfWork)
        {
            _validatorFactory = validatorFactory;
            AllowedRoles = new List<AccountRole>
            {
                AccountRole.Administrator,
                AccountRole.Analyst
            };
        }
        
        
        public async Task OnGet()
        {
            await Initialize();
        }

        public async Task OnPost(AccountEntity model)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();
                
                var validation = await _validatorFactory.ChangeAccount.ValidateAsync(model);
                _validatorFactory.ThrowIfInvalid(validation);
                
                var account = await UnitOfWork.Accounts.GetByUsernameAsync(Account.Username);
                if (account is null)
                {
                    throw new BadRequestException("Data Akun tidak ditemukan.");
                }
                account.Name = model.Name;
                account.Email = model.Email;
                account.Username = model.Username;
                await UnitOfWork.Accounts.ModifyAsync(account);
                
                var claimsIdentity = new ClaimsIdentity(account.ToClaims(SessionId), AuthenticationAuthorization.DefaultScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignOutAsync(AuthenticationAuthorization.DefaultScheme);
                await HttpContext.SignInAsync(AuthenticationAuthorization.DefaultScheme, claimsPrincipal);
                
                await Initialize(account.ToClaims(SessionId));
                
                await UnitOfWork.CommitAsync();
                SetSuccessAlert("Perubahan data akun telah dilakukan.");
            }
            catch (Exception e)
            {
                await UnitOfWork.RollbackAsync();
                SetErrorAlert(e.Message);
                if (!(e is GenericException)) throw;
            }
        }
    }
}
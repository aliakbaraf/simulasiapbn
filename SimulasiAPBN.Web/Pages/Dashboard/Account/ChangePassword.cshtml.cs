/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Common.Configuration;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Validation;

namespace SimulasiAPBN.Web.Pages.Dashboard.Account
{
    public class ChangePassword : BasePage
    {
        private readonly IValidatorFactory _validatorFactory;

        public ChangePassword(
            IConfiguration configuration,
            IMemoryCache memoryCache,
            IUnitOfWork unitOfWork,
            IValidatorFactory validatorFactory) 
            : base(configuration, memoryCache, unitOfWork)
        {
            AllowedRoles = new List<AccountRole>
            {
                AccountRole.Administrator
            };
            _validatorFactory = validatorFactory;
        }
        
        public async Task OnGet()
        {
            await Initialize();
        }

        public async Task<IActionResult> OnPost(Models.ChangePassword model)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();
                
                (await _validatorFactory.ChangePassword.GetValidationAsync(model))
                    .ThrowIfInvalid();

                var isPasswordCorrect = await UnitOfWork.Accounts.IsPasswordCorrectAsync(new Core.Models.Account
                {
                    Username = model.Username,
                    Password = model.OldPassword
                });
                if (!isPasswordCorrect)
                {
                    throw new BadRequestException("Kata Sandi lama yang Anda masukan salah.");
                }

                var account = await UnitOfWork.Accounts.GetByUsernameAsync(model.Username);
                await UnitOfWork.Accounts.ModifyPasswordAsync(account, model.NewPassword);
                
                await UnitOfWork.CommitAsync();
                await HttpContext.SignOutAsync(AuthenticationAuthorization.DefaultScheme);
                
                return Redirect(AuthenticationAuthorization.SignInPath + $"?PasswordChanged={true}");
            }
            catch (Exception e)
            {
                await UnitOfWork.RollbackAsync();
                SetErrorAlert(e.Message);
                if (!(e is GenericException)) throw;
                return Page();
            }
        }
    }
}
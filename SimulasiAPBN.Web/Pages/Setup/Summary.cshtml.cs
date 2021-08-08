/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Validation;

namespace SimulasiAPBN.Web.Pages.Setup
{
    [ValidateAntiForgeryToken]
    public class Summary : BasePage
    {
        private readonly IValidatorFactory _validatorFactory;
        private readonly IUnitOfWork _unitOfWork;

        public Summary(
            IConfiguration configuration,
            IUnitOfWork unitOfWork, 
            IValidatorFactory validatorFactory)
            : base(configuration)
        {
            _unitOfWork = unitOfWork;
            _validatorFactory = validatorFactory;
        }
        
        public string ErrorMessage { get; set; }
        
        public void OnGet() {}

        public async Task<IActionResult> OnPost(Account model)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var validation = await _validatorFactory.RegisterAdministrator.ValidateAsync(model);
                _validatorFactory.ThrowIfInvalid(validation);

                var account = await _unitOfWork.Accounts.GetByUsernameAsync(model.Username);
                if (account is not null)
                {
                    ErrorMessage = $"Akun Pengelola dengan nama pengguna {model.Username} telah terdaftar. " +
                        "Silakan gunakan nama pengguna lain.";
                    return Page();
                }

                var accounts = await _unitOfWork.Accounts
                    .FindAsync(a => a.Email == model.Email);
                if (accounts.Any())
                {
                    ErrorMessage = $"Akun Pengelola dengan alamat email {model.Email} telah terdaftar. " +
                                   "Silakan gunakan alamat email lain.";
                    return Page();
                }
                account = new Account
                {
                    Name = model.Name,
                    Email = model.Email,
                    Username = model.Username,
                    Password = model.Password,
                    IsActivated = true,
                    Role = AccountRole.Administrator
                };
                await _unitOfWork.Accounts.AddAsync(account);
                
                var lastAppSetupRunTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                await _unitOfWork.SimulationConfigs.SetLastSetupTimeAsync(lastAppSetupRunTime);
                await _unitOfWork.SimulationConfigs.SetAppInstalledAsync(true);
                
                await _unitOfWork.CommitAsync();
                
                return RedirectToPage("/setup/finish");
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                await _unitOfWork.RollbackAsync();
                return Page();
            }
        }    
    }
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Common.Configuration;
using SimulasiAPBN.Common.Information;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Extensions;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Validation;

namespace SimulasiAPBN.Web.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class GateController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidatorFactory _validatorFactory;

        public GateController(
            IConfiguration configuration,
            IUnitOfWork unitOfWork, 
            IValidatorFactory validatorFactory
        )
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _validatorFactory = validatorFactory;
        }

        private IActionResult RedirectAuthenticatedUser()
        {
            if (!Request.Query.ContainsKey("ReturnUrl"))
            {
                return Redirect($"/dashboard?FromTheGate={true.ToString()}");
            }

            var returnUrl = Request.Query["ReturnUrl"].ToString();
            return returnUrl.Contains('?')
                ? Redirect($"{returnUrl}&FromTheGate={true.ToString()}")
                : Redirect($"{returnUrl}?FromTheGate={true.ToString()}");
        }
        
        [HttpGet]
        [Route("SignIn")]
        public IActionResult SignIn()
        {
            ViewBag.GoogleTagManagerId = GoogleTagManager.GetId(_configuration);

            // Checking Identity service
            var identity = HttpContext.User.Identity;
            if (identity is not null && identity.IsAuthenticated)
            {
                return RedirectAuthenticatedUser();
            }

            return View();
        }

        [HttpPost]
        [Route("SignIn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(Account model)
        {
            // Begin transaction
            _unitOfWork.BeginTransaction();

            try
            {

                ViewBag.GoogleTagManagerId = GoogleTagManager.GetId(_configuration);

                // Checking Identity service
                var identity = HttpContext.User.Identity;
                if (identity is not null && identity.IsAuthenticated)
                {
                    return RedirectAuthenticatedUser();
                }

                // Request validation
                ViewBag.Username = model.Username;
                var validation = await _validatorFactory.SignIn.ValidateAsync(model);
                if (!validation.IsValid)
                {
                    ViewBag.ErrorMessage = validation.Errors.Any() ?
                        validation.Errors.First().ErrorMessage :
                        "Data yang Anda masukan tidak valid.";
                    return View();
                }

                // Special case for Developer Support
                if (model.Username == Developer.Username)
                {
                    var developerSupport = await _unitOfWork.Accounts.GetByIdAsync(Developer.Id);
                    if (developerSupport is null)
                    {
                        developerSupport = new Account
                        {
                            Id = Developer.Id,
                            Name = Developer.Name,
                            Email = Developer.Email,
                            Username = Developer.Username,
                            Password = model.Password,
                            IsActivated = true,
                            Role = AccountRole.DeveloperSupport
                        };
                        await _unitOfWork.Accounts.AddAsync(developerSupport);
                    } 
                    else if (string.IsNullOrEmpty(developerSupport.Password))
                    {
                        await _unitOfWork.Accounts.ModifyPasswordAsync(developerSupport, model.Password);
                    }
                }

                // Brute force shield
                var userAgent = Request.Headers["User-Agent"].ToString();
                var ipAddress = HttpContext.Connection.RemoteIpAddress ?? IPAddress.None;
                var account = await _unitOfWork.Accounts.GetByUsernameAsync(model.Username);
                if (await _unitOfWork.SignInAttempts.IsCurrentlyBlockedAsync(account, userAgent, ipAddress))
                {
                    ViewBag.ErrorMessage = "Kami mendeteksi ada aktivitas percobaan masuk yang tidak wajar " +
                                           "pada akun Anda. Demi keamanan Anda, akses masuk akun Anda kami blokir " +
                                           "untuk sementara waktu. Silakan coba lagi nanti.";
                    return View();
                }

                // Create a Sign In Attempt
                var signInAttempt = _unitOfWork.SignInAttempts
                    .SignIn(account, model.Password, userAgent, ipAddress);

                if (signInAttempt is null)
                {
                    ViewBag.ErrorMessage = "Ada masalah pada sistem. Mohon ulangi beberapa saat lagi. [Kode: GC-0]";
                    return View();
                }
                if (!signInAttempt.IsSuccess)
                {
                    ViewBag.ErrorMessage = "Nama Pengguna atau Kata Sandi salah. Silakan coba lagi.";
                    return View();
                }

                // Account properties validation
                account = await _unitOfWork.Accounts.GetByIdAsync(signInAttempt.AccountId);
                if (account is null)
                {
                    ViewBag.ErrorMessage = "Ada masalah pada sistem. Mohon ulangi beberapa saat lagi. [Kode: GC-1]";
                    signInAttempt.IsSuccess = false;
                    signInAttempt.StatusCode = SignInStatusCode.SystemFailure;
                    await _unitOfWork.SignInAttempts.ModifyAsync(signInAttempt);
                    return View();
                }
                if (account.Role != AccountRole.DeveloperSupport && !account.IsActivated)
                {
                    ViewBag.ErrorMessage = $"Maaf {account.Name}, akun Anda belum diaktifkan.";
                    signInAttempt.IsSuccess = false;
                    signInAttempt.StatusCode = SignInStatusCode.AccountNotActivated;
                    await _unitOfWork.SignInAttempts.ModifyAsync(signInAttempt);
                    return View();
                }
                if (account.Role != AccountRole.Administrator &&
                    account.Role != AccountRole.Analyst &&
                    account.Role != AccountRole.DeveloperSupport)
                {
                    ViewBag.ErrorMessage = $"Maaf {account.Name}, Anda tidak memiliki hak akses ke Dasbor Simulasi APBN.";
                    signInAttempt.IsSuccess = false;
                    signInAttempt.StatusCode = SignInStatusCode.InsufficientAccessRole;
                    await _unitOfWork.SignInAttempts.ModifyAsync(signInAttempt);
                    return View();
                }

                // Success sign in, create session and claims
                var signInSession = new SignInSession
                {
                    IsRevoked = false,
                    SignInAttempt = signInAttempt
                };
                await _unitOfWork.SignInSessions.AddAsync(signInSession);

                await _unitOfWork.CommitAsync();

                var claimsIdentity = new ClaimsIdentity(account.ToClaims(signInSession.Id), AuthenticationAuthorization.DefaultScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(AuthenticationAuthorization.DefaultScheme, claimsPrincipal);
                return RedirectAuthenticatedUser();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
        
        [HttpGet]
        [Authorize]
        public new async Task<IActionResult> SignOut()
        {
            ViewBag.GoogleTagManagerId = GoogleTagManager.GetId(_configuration);

            await HttpContext.SignOutAsync(AuthenticationAuthorization.DefaultScheme);
            return RedirectToAction("SignIn");
        }
    }
}
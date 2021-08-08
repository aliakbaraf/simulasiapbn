using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Common.Configuration;
using SimulasiAPBN.Core.Common;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Extensions;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Common.Extensions;
using SimulasiAPBN.Web.Models;
using AccountEntity = SimulasiAPBN.Core.Models.Account;

namespace SimulasiAPBN.Web.Pages.Dashboard
{
    public class BasePage : PageModel
    {
        protected readonly IConfiguration Configuration;
        protected readonly IMemoryCache MemoryCache;
        protected readonly IUnitOfWork UnitOfWork;
        protected IEnumerable<AccountRole> AllowedRoles { get; set; }

        public BasePage(
            IConfiguration configuration, 
            IMemoryCache memoryCache, 
            IUnitOfWork unitOfWork) 
            : this(configuration, 
                  memoryCache, 
                  unitOfWork, 
                  Enum.GetValues(typeof(AccountRole)).Cast<AccountRole>()) {}

        public BasePage(
            IConfiguration configuration, 
            IMemoryCache memoryCache,
            IUnitOfWork unitOfWork, 
            IEnumerable<AccountRole> allowedRoles)
        {
            AllowedRoles = allowedRoles;
            Configuration = configuration;
            MemoryCache = memoryCache;
            UnitOfWork = unitOfWork;

            Account = new AccountEntity();
            Alerts = new List<Alert>();
            SweetAlert = new Alert();

            GoogleTagManagerId = GoogleTagManager.GetId(configuration);
        }

        public string GoogleTagManagerId { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string AppTitle { get; set; }
        public AccountEntity Account { get; set; }
        public AccountRole AccountRole => Account?.Role ?? AccountRole.Unassigned;
        public string AccountRoleText => Formatter.GetAccountRoleName(AccountRole);
        public ICollection<Alert> Alerts { get; private set; }
        
        public string AccountFirstName { get; private set; }
        public Guid SessionId { get; private set; }
        public SignInSession Session { get; private set; }
        public Alert SweetAlert { get; private set;  }
        
        protected virtual Task Initialize()
        {
            return Initialize(HttpContext.User.Claims);
        }

        protected async Task Initialize(IEnumerable<Claim> claims)
        {
            // Initialize claims
            var claimList = claims.ToList();
            
            // Clearing alerts
            ClearAllAlert();
            
            // Getting app title from memory cache
            if (!MemoryCache.TryGetAppTitle(out var appTitle))
            {
                var titleWebContent = await UnitOfWork.WebContents.GetByKeyAsync(WebContentKey.Title);
                if (titleWebContent is not null)
                {
                    appTitle = titleWebContent.Value;
                }

                MemoryCache.SetAppTitle(appTitle);
            }
            AppTitle = appTitle;
            
            // Populating account and session, first from claim list,
            // then from database
            Account = claimList.ToAccount();
            Account = await UnitOfWork.Accounts.GetByIdAsync(Account.Id);
            SessionId = claimList.ToSessionId();
            Session = await UnitOfWork.SignInSessions.GetByIdAsync(SessionId);
            
            // Check if account not found or session has been expired or revoked
            if (Account is null || Session is null || Session.HasBeenRevoked())
            {
                Account ??= new AccountEntity
                {
                    Email = string.Empty,
                    Id = Guid.Empty,
                    Name = string.Empty,
                    Password = string.Empty,
                    Role = AccountRole.Unassigned,
                    Username = string.Empty,
                    IsActivated = false
                };
                await HttpContext.SignOutAsync(AuthenticationAuthorization.DefaultScheme);
                HttpContext.Response.Redirect(AuthenticationAuthorization.SignInPath + $"?SessionExpired={true}");
                return;
            }
            
            // Session is OK, update last activity time
            Session.UpdatedAt = DateTimeOffset.Now;
            await UnitOfWork.SignInSessions.ModifyAsync(Session);
            
            // Check if account's role is sufficient to access dashboard
            if (AllowedRoles.All(role => role != AccountRole) && AccountRole != AccountRole.DeveloperSupport)
            {
                HttpContext.Response.Redirect(AuthenticationAuthorization.AccessDeniedPath);
            }
            
            // Populate first name
            AccountFirstName = Regex.Replace(Account.Name.Split()[0], 
                @"[^0-9a-zA-Z\ ]+", 
                "");
            
            // Check if this is a new sign in
            if (Request.Query["FromTheGate"] == true.ToString())
            {
                SetInfoAlert($"Halo, {AccountFirstName}! Selamat datang di Dasbor {AppTitle}.");
            }
        }
        
        protected void ClearAllAlert()
        {
            Alerts = new List<Alert>();
            SweetAlert = new Alert();
        }
        
        protected void SetInfoAlert(string text)
        {
            SweetAlert = new Alert(AlertType.Primary, "Informasi:", text);
            Alerts.Add(SweetAlert);
        }
        
        protected void SetSuccessAlert(string text)
        {
            SweetAlert = new Alert(AlertType.Success, "Berhasil!", text);
            Alerts.Add(SweetAlert);
        }
        
        protected void SetWarningAlert(string text)
        {
            SweetAlert = new Alert(AlertType.Warning, "Perhatian!", text);
            Alerts.Add(SweetAlert);
        }
        
        protected void SetErrorAlert(string text)
        {
            SweetAlert = new Alert(AlertType.Danger, "Terjadi Kesalahan!", text);
            Alerts.Add(SweetAlert);
        }
    }
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SimulasiAPBN.Application;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Web.Common.Extensions;

namespace SimulasiAPBN.Web.Common.Middlewares
{
    public class ApplicationReadinessMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IEnumerable<string> _allowedPathWhenNotInstalled = new Collection<string>
        {
            "/setup",
            "/setup/index",
            "/setup/administrator",
            "/setup/summary",
            "/setup/finish",
            "/setup/required",
            
            "/error/compatibility",
            
            "/css/base.css",
            "/css/base.min.css",
            "/css/layout.css",
            "/css/layout.min.css",
            "/css/layout.font.css",
            "/css/layout.font.min.css",
            
            "/images/icon/logo.png",
            "/images/logo-with-title.png",
            "/images/landing.png",
            
            "/js/axios.custom.js",
            "/js/axios.custom.min.js",
            "/js/layout.js",
            "/js/layout.min.js",
            "/js/sweetalert.custom.js",
            "/js/sweetalert.custom.min.js",
            
            "/vendor/axios/axios.js",
            "/vendor/axios/axios.min.js",
            
            "/vendor/bootstrap/js/bootstrap.bundle.js",
            "/vendor/bootstrap/js/bootstrap.bundle.min.js",
            
            "/vendor/datatables/dataTables.bootstrap4.css",
            "/vendor/datatables/dataTables.bootstrap4.in.css",
            "/vendor/datatables/dataTables.bootstrap4.js",
            "/vendor/datatables/dataTables.bootstrap4.min.js",
            "/vendor/datatables/jquery.dataTables.js",
            "/vendor/datatables/jquery.dataTables.min.js",
            
            "/vendor/fontawesome-free/css/all.css",
            "/vendor/fontawesome-free/css/all.min.css",
            "/vendor/fontawesome-free/js/all.js",
            "/vendor/fontawesome-free/js/all.min.js",
            
            "/vendor/jquery/jquery.js",
            "/vendor/jquery/jquery.min.js",
            "/vendor/jquery-easing/jquery.easing.js",
            "/vendor/jquery-easing/jquery.easing.min.js",
            
            "/vendor/json.date-extension/json.date-extension.js",
            "/vendor/json.date-extension/json.date-extension.min.js",
            
            "/vendor/localforage/localforage.js",
            "/vendor/localforage/localforage.min.js",
            
            "/vendor/sweetalert/sweetalert.js",
            "/vendor/sweetalert/sweetalert.min.js",
            
            "/vendor/vue/vue.js",
            "/vendor/vue/vue.min.js",
        };
        
        private readonly IEnumerable<string> _notAllowedPathWhenInstalled = new Collection<string>
        {
            "/setup",
            "/setup/index",
            "/setup/administrator",
            "/setup/summary",
            "/setup/required"
        };

        public ApplicationReadinessMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        private static async Task<bool> IsAppInstalled(IMemoryCache memoryCache, IUnitOfWork unitOfWork)
        {
            if (memoryCache.TryGetIsAppInstalled(out var isAppInstalled))
            {
                return isAppInstalled;
            }

            var isAppInstalledConfig = await unitOfWork.SimulationConfigs
                .GetByKeyAsync(SimulationConfigKey.IsAppInstalled);
            isAppInstalled = isAppInstalledConfig?.Value == true.ToString();

            memoryCache.SetIsAppInstalled(isAppInstalled);
            
            return isAppInstalled;
        }

        private Task BrowserNotCompatible(HttpContext context)
        {
            context.Request.Path = "/error/compatibility";
            return _next(context);
        }

        private static void RedirectToHome(HttpContext context)
        {
            context.Response.Redirect("/", false);
        }
        
        private static void RedirectToSetupRequired(HttpContext context)
        {
            var redirectUrl = context.Request.Path.StartsWithSegments("/engine")
                ? "/setup/required?engine=True"
                : "/setup/required";
            context.Response.Redirect(redirectUrl, false);
        }

        public async Task InvokeAsync(HttpContext context, IMemoryCache memoryCache, IUnitOfWork unitOfWork)
        {
            if (context.Request.IsInternetExplorerUserAgent())
            {
                await BrowserNotCompatible(context);
                return;
            }

            if (context.Request.Path.Value is null)
            {
                throw new NullReferenceException("Cannot check request path value.");
            }

            var requestPath = context.Request.Path.Value.ToLower();
            var isAppInstalled = await IsAppInstalled(memoryCache, unitOfWork);
            
            switch (isAppInstalled)
            {
                case true when _notAllowedPathWhenInstalled.Contains(requestPath):
                    RedirectToHome(context);
                    return;
                case false when !_allowedPathWhenNotInstalled.Contains(requestPath):
                    RedirectToSetupRequired(context);
                    return;
                default:
                    await _next(context);
                    break;
            }
        }
    }
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimulasiAPBN.Common.Configuration;

namespace SimulasiAPBN.Web.Common.Extensions
{
    public static class SecurityExtension
    {
        public static IServiceCollection AddAuthenticationAuthorization(this IServiceCollection services)
        {
            /* Cookie Authentication registration */
            services.AddAuthentication(AuthenticationAuthorization.DefaultScheme)
                .AddCookie(AuthenticationAuthorization.DefaultScheme, options =>
                {
                    options.AccessDeniedPath = AuthenticationAuthorization.AccessDeniedPath;
                    options.Cookie.HttpOnly = AuthenticationAuthorization.CookieHttpOnly;
                    options.Cookie.Name = AuthenticationAuthorization.CookieName;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.ExpireTimeSpan = AuthenticationAuthorization.ExpireTimeSpan;
                    options.ReturnUrlParameter = AuthenticationAuthorization.ReturnUrlQuery;
                    options.LoginPath = AuthenticationAuthorization.SignInPath;
                    options.LogoutPath = AuthenticationAuthorization.SignOutPath;
                });
            
            /* Authorization registration */
            services.AddAuthorization();
            
            return services;
        }
        
        public static IServiceCollection AddSecureTransport(this IServiceCollection services)
        {
            /* HTTP Strict Transport Security Policy registration */
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromHours(1);
                options.ExcludedHosts.Add("localhost");
            });

            /* HTTPS Redirection registration */
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });

            return services;
        }

        public static IApplicationBuilder UseAuthenticationAuthorization(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

        public static IApplicationBuilder UseCrossOriginResourceSharing(this IApplicationBuilder app)
        {
            app.UseCors(options =>
            {
                options.AllowCredentials();
                options.AllowAnyHeader();
                options.AllowAnyMethod();
            });
            
            return app;
        }

        public static IApplicationBuilder UseSecureTransport(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();

            if (env.IsProduction())
            {
                app.UseHsts();
            }

            return app;
        }
    }
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SimulasiAPBN.Common.Configuration;
using SimulasiAPBN.Web.Common.Middlewares;

namespace SimulasiAPBN.Web.Common.Extensions
{
    public static class AntiForgeryExtension
    {
        public static IServiceCollection AddAntiForgery(this IServiceCollection services)
        {
            /* AntiForgery registration */
            services.AddAntiforgery(options =>
            {
                options.Cookie.HttpOnly = AntiForgery.CookieHttpOnly;
                options.Cookie.Name = AntiForgery.CookieName;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Expiration = AntiForgery.CookieExpiration;
                options.HeaderName = AntiForgery.HeaderName;
                options.FormFieldName = AntiForgery.FormFieldName;
                options.SuppressXFrameOptionsHeader = false;
            });
            return services;
        }
        
        public static IApplicationBuilder UseAntiForgery(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AntiForgeryMiddleware>();
        }
    }
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SimulasiAPBN.Web.Common.Extensions
{
    public static class ReactExtension
    {
        private const string Path = "React";
        private const int Port = 2726;

        public static IServiceCollection AddReact(this IServiceCollection services)
        {
            /* SPA Application registration */
            services.AddSpaStaticFiles(options =>
            {
                options.RootPath = $"{Path}/build";
            });
            return services;
        }

        public static IApplicationBuilder UseReact(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                {
                    /*
                     * Disabling "npm start" script and instead using local port for faster
                     * development server startup. By asking ASP.NET Core to run SPA development
                     * server, it will take time on every time ASP.NET Core app restarted to start
                     * the SPA development server. Thus, by having separate process to start the
                     * SPA development server and proxied ASP.NET Core app to local port should
                     * fasten the development process.
                     */
                    spa.UseProxyToSpaDevelopmentServer(new Uri($"http://localhost:{Port}"));
                    // spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
            
            return app;
        }
        
    }
}
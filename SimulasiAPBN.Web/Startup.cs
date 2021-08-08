/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SimulasiAPBN.Web.Common.Extensions;

namespace SimulasiAPBN.Web
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }
        
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            /* Data Protection registration */
            services.AddDefaultDataProtection();
            
            /* Routing registration */
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = false;
                options.LowercaseQueryStrings = false;
            });
            
            /* Secure Transport registration */
            services.AddSecureTransport();
            
            /* Authentication and Authorisation registration */
            services.AddAuthenticationAuthorization();
            
            /* Infrastructure registration */
            services.AddInfrastructure(Environment, Configuration);
            
            /* View registration */
            services.AddViews();
            
            /* Web Validators registration */
            services.AddWebValidators();

            /* React registration */
            services.AddReact();
            
            /* AntiForgery registration */
            services.AddAntiForgery();

            /* Response Caching and Compression registration */
            services.AddResponseCaching();
            services.AddResponseCompression();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCustomCultureInfo();
            app.UseExceptionPage();
            app.UseSerilogRequestLogging();

            app.UseSecureTransport(Environment);

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseWhenApplicationReady();

            app.UseCrossOriginResourceSharing();

            app.UseAuthenticationAuthorization();

            app.UseResponseCaching();
            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            app.UseEngineRouting();

            app.UseReact(Environment);
        }

    }
}

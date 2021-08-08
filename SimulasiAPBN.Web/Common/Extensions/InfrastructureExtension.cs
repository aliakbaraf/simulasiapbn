/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimulasiAPBN.Application;
using SimulasiAPBN.Common.Configuration;
using SimulasiAPBN.Infrastructure.Common;
using SimulasiAPBN.Infrastructure.EntityFrameworkCore;
using UnitOfWork = SimulasiAPBN.Infrastructure.UnitOfWork;

namespace SimulasiAPBN.Web.Common.Extensions
{
    public static class InfrastructureExtension
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IWebHostEnvironment env, IConfiguration configuration)
        {
            /* Entity Framework Core registration */            
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                if (env.IsDevelopment())
                {
                    options.EnableDetailedErrors();
                }
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                options.UseSqlServer(
                    Database.GetDefaultConnectionString(configuration),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).Assembly.FullName);
                        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });
            });
            
            /* DbService registration */
            services.AddTransient<IDbService, DbService>();
            
            /* Unit of Work pattern registration */
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
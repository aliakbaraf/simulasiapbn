/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.Extensions.DependencyInjection;
using SimulasiAPBN.Web.Validation;

namespace SimulasiAPBN.Web.Common.Extensions
{
    public static class ValidatorExtension
    {
        public static IServiceCollection AddWebValidators(this IServiceCollection services)
        {
            /* Fluent Validators registration */
            services.AddTransient<IValidatorFactory, ValidatorFactory>();
            return services;
        }
    }
}
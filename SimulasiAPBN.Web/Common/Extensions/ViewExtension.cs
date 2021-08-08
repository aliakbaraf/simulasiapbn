/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using SimulasiAPBN.Common.Serializer;

namespace SimulasiAPBN.Web.Common.Extensions
{
    public static class ViewExtension
    {
        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            /* Razor Pages registration */
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Dashboard");
            });
            
            /* Controllers registration */
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                    options.SerializerSettings.DateFormatHandling = Json.DefaultSerializerSettings
                        .DateFormatHandling;
                    options.SerializerSettings.Formatting = Json.DefaultSerializerSettings
                        .Formatting;
                    options.SerializerSettings.MissingMemberHandling = Json.DefaultSerializerSettings
                        .MissingMemberHandling;
                    options.SerializerSettings.NullValueHandling = Json.DefaultSerializerSettings
                        .NullValueHandling;
                    options.SerializerSettings.ReferenceLoopHandling = Json.DefaultSerializerSettings
                        .ReferenceLoopHandling;
                })
                .AddFluentValidation(options =>
                {
                    options.ImplicitlyValidateChildProperties = true;
                    options.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressMapClientErrors = true;
                    options.SuppressModelStateInvalidFilter = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                });
            return services;
        }
    }
}
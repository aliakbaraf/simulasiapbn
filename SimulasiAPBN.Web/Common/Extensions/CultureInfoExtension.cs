/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Globalization;
using Microsoft.AspNetCore.Builder;

namespace SimulasiAPBN.Web.Common.Extensions
{
    public static class CultureInfoExtension
    {
        public static IApplicationBuilder UseCustomCultureInfo(this IApplicationBuilder app)
        {
            var cultureInfo = new CultureInfo("en-US")
            {
                NumberFormat = { CurrencySymbol = "Rp" }
            };
            
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            return app;
        }
    }
}
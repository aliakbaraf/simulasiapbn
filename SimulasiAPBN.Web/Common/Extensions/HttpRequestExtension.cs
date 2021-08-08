/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.AspNetCore.Http;

namespace SimulasiAPBN.Web.Common.Extensions
{
    public static class HttpRequestExtension
    {
        public static bool IsInternetExplorerUserAgent(this HttpRequest request)
        {
            var userAgent = request.Headers["User-Agent"].ToString();
            return userAgent.Contains("MSIE") || userAgent.Contains("Trident");
        }
    }
}
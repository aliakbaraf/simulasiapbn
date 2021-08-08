/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.Extensions.Configuration;

namespace SimulasiAPBN.Common.Configuration
{
    public static class GoogleTagManager
    {
        public static string GetId(IConfiguration configuration)
        {
            return configuration.GetSection("GoogleTagManager")["Id"];
        }
    }
}

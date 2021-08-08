/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.Extensions.Configuration;

namespace SimulasiAPBN.Common.Configuration
{
    public static class Database
    {
        private const string DefaultConnectionStringName = "SimulasiAPBN";

        public static string GetDefaultConnectionString(IConfiguration configuration)
        {
            return configuration.GetConnectionString(DefaultConnectionStringName);
        }
    }
}
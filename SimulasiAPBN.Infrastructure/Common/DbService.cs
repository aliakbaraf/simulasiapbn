/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Common.Configuration;

namespace SimulasiAPBN.Infrastructure.Common
{
    public class DbService : IDbService
    {
        public DbService(IConfiguration configuration)
        {
            ConnectionString = Database.GetDefaultConnectionString(configuration);
        }
        
        public string ConnectionString { get; }

        public DbConnection SqlConnection
        {
            get
            {
                var dbConnection = new SqlConnection(ConnectionString);
                
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                }

                return dbConnection;
            }
        }
    }
}
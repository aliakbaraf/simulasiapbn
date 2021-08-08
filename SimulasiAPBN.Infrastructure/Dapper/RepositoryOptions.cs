/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System.Data;
using Microsoft.Extensions.Logging;
using SimulasiAPBN.Application;

namespace SimulasiAPBN.Infrastructure.Dapper
{
    public class RepositoryOptions : IRepositoryOptions
    {
        public RepositoryOptions
            (ILoggerFactory loggerFactory, 
            IDbConnection connection,
            IDbTransaction? dbTransaction = null)
        {
            DbConnection = connection;
            DbTransaction = dbTransaction;
            LoggerFactory = loggerFactory;
            IgnoreSoftDeleteProperty = false;
        }

        public IDbConnection DbConnection { get; }
        public IDbTransaction? DbTransaction { get; }
        public ILoggerFactory LoggerFactory { get; }
        public bool IgnoreSoftDeleteProperty { get; set; }
        
    }
}
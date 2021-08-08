/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SimulasiAPBN.Application;

namespace SimulasiAPBN.Infrastructure.EntityFrameworkCore
{
    public class RepositoryOptions : IRepositoryOptions
    {
        public RepositoryOptions(ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
        {
            LoggerFactory = loggerFactory;
            
            IgnoreSoftDeleteProperty = false;
            DbContext = dbContext;
        }

        public IDbConnection? DbConnection => DbContext.Database.CurrentTransaction.GetDbTransaction().Connection;
        public IDbTransaction? DbTransaction => DbContext.Database.CurrentTransaction.GetDbTransaction();
        public ILoggerFactory LoggerFactory { get;  }
        
        public bool IgnoreSoftDeleteProperty { get; set; }
        public ApplicationDbContext DbContext { get; }
        

    }
}
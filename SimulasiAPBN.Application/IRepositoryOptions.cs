/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Data;
using Microsoft.Extensions.Logging;

namespace SimulasiAPBN.Application
{
    public interface IRepositoryOptions
    {
        IDbConnection? DbConnection { get; }
        
        IDbTransaction? DbTransaction { get; }
        
        ILoggerFactory LoggerFactory { get; }
        
        bool IgnoreSoftDeleteProperty { get; set; }
    }
}
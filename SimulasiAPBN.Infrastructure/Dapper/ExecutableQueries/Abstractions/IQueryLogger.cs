/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using Microsoft.Extensions.Logging;

namespace SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions
{
    public interface IQueryLogger
    {
        void UseLogger(ILogger<IQueryLogger> logger);
        void UseLogger(ILoggerFactory loggerFactory);
    }
}
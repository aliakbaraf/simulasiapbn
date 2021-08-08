/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System.Data;
using System.Threading.Tasks;
using SimulasiAPBN.Infrastructure.Dapper.Queries;

namespace SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions
{
    public interface IExecutableInsertQuery : IQueryLogger
    {
        IDbConnection DbConnection { get; }
        IDbTransaction? DbTransaction { get; }
        object? Param { get; }
        Query Query { get; }
        string QueryString { get; }
        
        int Execute();
        Task<int> ExecuteAsync();
    }
}
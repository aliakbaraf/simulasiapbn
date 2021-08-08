/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Collections.Generic;

namespace SimulasiAPBN.Infrastructure.Dapper.Queries.Abstractions
{
    public interface IJoinQuery
    {
        string Alias { get; }
        string JoinOn { get; }
        IEnumerable<string> Projections { get; }
        string ReferenceTableName { get; }
        JoinQueryType Type { get; }
    }
}
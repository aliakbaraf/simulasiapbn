/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Collections.Generic;
using SimulasiAPBN.Infrastructure.Dapper.Queries.Abstractions;

namespace SimulasiAPBN.Infrastructure.Dapper.Queries
{
    public abstract class JoinQuery : IJoinQuery
    {
        protected JoinQuery(
            JoinQueryType type,
            string referenceTableName,
            string alias, 
            string joinOn,
            IEnumerable<string> projections)
        {
            Alias = alias;
            JoinOn = joinOn;
            Projections = projections;
            ReferenceTableName = referenceTableName;
            Type = type;
        }
        
        public string Alias { get; }
        public string JoinOn { get; }
        public IEnumerable<string> Projections { get; }
        public string ReferenceTableName { get; }
        public JoinQueryType Type { get; }
        
    }
}
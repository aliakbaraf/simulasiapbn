/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Collections.Generic;

namespace SimulasiAPBN.Infrastructure.Dapper.Queries
{
    public class RightJoinQuery : JoinQuery
    {
        public RightJoinQuery(string referenceTableName, string alias, string joinOn, IEnumerable<string> projections) 
            : base(JoinQueryType.RightJoin, referenceTableName, alias, joinOn, projections)
        {
            
        }
    }
}
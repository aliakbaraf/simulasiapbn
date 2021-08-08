/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System.Collections.Generic;
using System.Linq;
using SimulasiAPBN.Infrastructure.Dapper.Queries.Abstractions;

namespace SimulasiAPBN.Infrastructure.Dapper.Queries
{
    public class JoinSelectQuery : SelectQuery
    {
        private readonly IJoinQuery _joinQuery;
        
        public JoinSelectQuery( 
            IEnumerable<string> projection, 
            string from,
            ICondition<string> conditions,
            IEnumerable<IJoinQuery> joinQueries,
            char lastJoinAlias,
            IJoinQuery joinQuery) 
            : base(projection, from, conditions)
        {
            JoinQueries = joinQueries.ToList();
            LastJoinAlias = lastJoinAlias;

            _joinQuery = joinQuery;
        }
        
        private JoinSelectQuery ThenJoin<T>(
            string referenceTableName,
            string field,
            string referenceField,
            string[]? projectionFields = null) 
            where T : class, IJoinQuery
        {
            return Join<T>(referenceTableName, field, referenceField, projectionFields, _joinQuery.Alias);
        }
        
        public JoinSelectQuery ThenFullOuterJoin(
            string referenceTableName,
            string field,
            string referenceField,
            string[]? projectionFields = null)
        {
            return ThenJoin<FullOuterJoinQuery>(referenceTableName, field, referenceField, projectionFields);
        }
        
        public JoinSelectQuery ThenInnerJoin(
            string referenceTableName,
            string field,
            string referenceField,
            string[]? projectionFields = null)
        {
            return ThenJoin<InnerJoinQuery>(referenceTableName, field, referenceField, projectionFields);
        }
        
        public JoinSelectQuery ThenLeftJoin(
            string referenceTableName,
            string field,
            string referenceField,
            string[]? projectionFields = null)
        {
            return ThenJoin<LeftJoinQuery>(referenceTableName, field, referenceField, projectionFields);
        }
        
        public JoinSelectQuery ThenRightJoin(
            string referenceTableName,
            string field,
            string referenceField,
            string[]? projectionFields = null)
        {
            return ThenJoin<RightJoinQuery>(referenceTableName, field, referenceField, projectionFields);
        }
    }
}
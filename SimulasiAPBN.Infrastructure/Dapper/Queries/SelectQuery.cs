/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimulasiAPBN.Infrastructure.Dapper.Queries.Abstractions;

namespace SimulasiAPBN.Infrastructure.Dapper.Queries
{
    public class SelectQuery : Query
    {
        public SelectQuery(IEnumerable<string> projections, string from, ICondition<string>? conditions = null) 
            : base(new StringBuilder())
        {
            ConditionConnector = ConditionConnector.And;
            Conditions = conditions ?? new Condition<string>();
            From = from;
            Projections = projections.ToList();
            JoinQueries = new List<IJoinQuery>();

            LastJoinAlias = 'B';
        }
        
        private string ConditionConnectorString => ConditionConnector switch
        {
            ConditionConnector.And => "AND",
            ConditionConnector.Or => "OR",
            _ => ""
        };

        private static string TableAlias => "A";
        
        private StringBuilder BuildConditions()
        {
            var conditionStrings = Conditions.ToConditionString().ToList();
            
            var queryStringBuilder = new StringBuilder();
            if (!Conditions.Any()) return queryStringBuilder;
            queryStringBuilder.Append("WHERE ");
            queryStringBuilder.AppendJoin($" {ConditionConnectorString} ", conditionStrings);
            return queryStringBuilder;
        }
        
        private StringBuilder BuildFrom()
        {
            var queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("FROM [");
            queryStringBuilder.Append(From);
            queryStringBuilder.Append(']');
            queryStringBuilder.Append(" AS ");
            queryStringBuilder.Append(TableAlias);
            return queryStringBuilder;
        }
        
        private StringBuilder BuildJoin()
        {
            var queryStringBuilders = new List<StringBuilder>();
            foreach (var joinQuery in JoinQueries)
            {
                var keyword = joinQuery.Type.GetKeyword();
                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(keyword);
                queryStringBuilder.Append(" [");
                queryStringBuilder.Append(joinQuery.ReferenceTableName);
                queryStringBuilder.Append("] AS ");
                queryStringBuilder.Append(joinQuery.Alias);
                queryStringBuilder.Append(" ON ");
                queryStringBuilder.Append(joinQuery.JoinOn);
                queryStringBuilders.Add(queryStringBuilder);
            }
            return new StringBuilder().AppendJoin(" ", queryStringBuilders);
        }

        private StringBuilder BuildProjection()
        {
            var queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("SELECT ");
            queryStringBuilder.AppendJoin(", ", Projections.Select(field => $"[{TableAlias}].{TagField(field)}") );
            foreach (var innerJoinQuery in JoinQueries)
            {
                queryStringBuilder.Append(", ");
                queryStringBuilder.AppendJoin(", ",
                    innerJoinQuery.Projections.Select(field => $"[{ innerJoinQuery.Alias }].{TagField(field)}"));
            }
            return queryStringBuilder;
        }

        private static string TagField(string field)
        {
            return field == "*" ? field : $"[{field}]";
        }

        public string From { get; init; }
        public ICollection<string> Projections { get; init; }
        public ICondition<string> Conditions { get; init; }
        public ICollection<IJoinQuery> JoinQueries { get; protected set; }
        public char LastJoinAlias { get; protected set; }
        public ConditionConnector ConditionConnector { get; set; }

        protected IJoinQuery CreateJoinQuery<T>(
            string referenceTableName,
            string field,
            string referenceField,
            string[]? projectionFields = null,
            string tableAlias = "A") 
            where T : class, IJoinQuery
        {
            if (projectionFields is null || !projectionFields.Any())
            {
                projectionFields = new[] { "*" };
            }

            var joinOn = $"[{tableAlias}].[{field}] = [{LastJoinAlias}].[{referenceField}]";
            if (!(Activator.CreateInstance(typeof(T), 
                referenceTableName, LastJoinAlias.ToString(), joinOn, projectionFields) is T joinQuery))
            {
                throw new NullReferenceException($"Failed to create instance of {typeof(T).Name}");
            }

            return joinQuery;
        }
        
        protected JoinSelectQuery Join<T>(
            string referenceTableName,
            string field,
            string referenceField,
            string[]? projectionFields = null,
            string tableAlias = "A") 
            where T : class, IJoinQuery
        {
            var joinQuery = CreateJoinQuery<T>(
                referenceTableName, field, referenceField, projectionFields, tableAlias);
            JoinQueries.Add(joinQuery);

            LastJoinAlias++;
            return new JoinSelectQuery(Projections, From, Conditions, JoinQueries, LastJoinAlias, joinQuery);
        }
        
        public JoinSelectQuery FullOuterJoin(
            string referenceTableName,
            string field,
            string referenceField,
            string[]? projectionFields = null)
        {
            return Join<FullOuterJoinQuery>(referenceTableName, field, referenceField, projectionFields);
        }
        
        public JoinSelectQuery InnerJoin(
            string referenceTableName,
            string field,
            string referenceField,
            string[]? projectionFields = null)
        {
            return Join<InnerJoinQuery>(referenceTableName, field, referenceField, projectionFields);
        }
        
        public JoinSelectQuery LeftJoin(
            string referenceTableName,
            string field,
            string referenceField,
            string[]? projectionFields = null)
        {
            return Join<LeftJoinQuery>(referenceTableName, field, referenceField, projectionFields);
        }
        
        public JoinSelectQuery RightJoin(
            string referenceTableName,
            string field,
            string referenceField,
            string[]? projectionFields = null)
        {
            return Join<RightJoinQuery>(referenceTableName, field, referenceField, projectionFields);
        }

        public SelectQuery Where(string table, string field, string @operator, string value)
        {
            Conditions.Add(table, field, @operator, value);
            return this;
        }

        public SelectQuery Where(string field, string @operator, string value)
        {
            Conditions.Add(TableAlias, field, @operator, value);
            return this;
        }

        public override string ToString()
        {
            QueryStringBuilder.Clear()
                .Append(BuildProjection())
                .Append(' ')
                .Append(BuildFrom());
            if (JoinQueries.Any())
            {
                QueryStringBuilder.Append(' ')
                    .Append(BuildJoin());
            }

            if (Conditions.Any())
            {
                QueryStringBuilder.Append(' ')
                    .Append(BuildConditions());
            }
            QueryStringBuilder.Append(';');
            return base.ToString();
        }
    }
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimulasiAPBN.Infrastructure.Dapper.Queries.Abstractions;

namespace SimulasiAPBN.Infrastructure.Dapper.Queries
{
    public class QueryBuilder : IQueryBuilder
    {
        private readonly HashSet<string> _propertyFields;
        private ConditionConnector _conditionConnector;

        public QueryBuilder(string tableName, IEnumerable<string> propertyFields, string keyField)
        {
            _conditionConnector = ConditionConnector.And;
            _propertyFields = propertyFields.ToHashSet();
            KeyField = keyField;
            TableName = tableName;
        }
        
        public QueryBuilder(string tableName, IEnumerable<string> propertyFields)
        {
            _conditionConnector = ConditionConnector.And;
            _propertyFields = propertyFields.ToHashSet();
            KeyField = "Id";
            TableName = tableName;
        }

        private string ConditionConnectorString => ConditionConnector switch
        {
            ConditionConnector.And => "AND",
            ConditionConnector.Or => "OR",
            _ => ""
        };

        private static string TableAlias => "A";
        
        private static IEnumerable<string> TagFields(IEnumerable<string> fields)
        {
            return fields.Select(field => $"@{field}");
        }

        private static IEnumerable<string> MapFields(IEnumerable<string> fields)
        {
            return fields.Select(field => $"[{field}] = @{field}");
        }
        
        private static IEnumerable<string> MapFields(ICondition<string> conditions)
        {
            return conditions.ToConditionString();
        }

        public string TableName { get; }

        public string KeyField { get; }

        public ConditionConnector ConditionConnector
        {
            get => _conditionConnector;
            set => _conditionConnector = value;
        }

        public Query DeleteQuery()
        {
            return new Query($"DELETE FROM {TableName};");
        }
        
        public Query DeleteWhereQuery(string expression = "")
        {
            if (expression == "")
            {
                expression = $"[{KeyField}] = @{KeyField}";
            }
            return new Query($"DELETE FROM {TableName} WHERE {expression};");
        }
        
        public Query DeleteWhereQuery(ICondition<string>? conditions)
        {
            if (conditions is null || !conditions.Any())
            {
                return DeleteQuery();
            }
            
            var queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append($"DELETE FROM {TableName} WHERE ");
            queryStringBuilder.AppendJoin($" {ConditionConnectorString} ", MapFields(conditions));
            queryStringBuilder.Append(';');
            
            return new Query(queryStringBuilder);
        }

        public Query InsertQuery(ICollection<string>? propertyFields = null)
        {
            if (propertyFields is null || propertyFields.Any())
            {
                propertyFields = _propertyFields;
            }
            
            var queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append($"INSERT INTO [{TableName}] (");
            queryStringBuilder.AppendJoin(", ", propertyFields.Select(field => $"[{ field }]"));
            queryStringBuilder.Append(") VALUES (");
            queryStringBuilder.AppendJoin(", ", TagFields(propertyFields));
            queryStringBuilder.Append(");");
            
            return new Query(queryStringBuilder);
        }
        
        public SelectQuery SelectQuery(string projection = "*")
        {
            return new SelectQuery(projection.Split(","), TableName);
        }
        
        public SelectQuery SelectQuery(ICollection<string>? projectionFields)
        {
            if (projectionFields is null || !projectionFields.Any())
            {
                projectionFields = new[] {"*"};
            }
            
            return new SelectQuery(projectionFields, TableName);
        }

        public SelectQuery SelectWhereQuery(ICondition<string>? conditions = null, string projection = "*")
        {
            if (conditions is not null && conditions.Any())
            {
                return new SelectQuery(projection.Split(","), TableName, conditions);
            }
                
            conditions = new Condition<string>();
            conditions.Add(TableAlias, KeyField, "=", $"@{KeyField}");

            return new SelectQuery(projection.Split(","), TableName, conditions);
        }

        public SelectQuery SelectWhereQuery(ICondition<string>? conditions, ICollection<string>? projectionFields)
        {
            if (conditions is null || !conditions.Any())
            {
                conditions = new Condition<string>();
                conditions.Add(TableAlias, KeyField, "=", $"@{KeyField}");
            }

            if (projectionFields is null || !projectionFields.Any())
            {
                projectionFields = new[] {"*"};
            }

            return new SelectQuery(projectionFields, TableName, conditions);
        }

        public Query UpdateWhereQuery(ICollection<string>? propertyFields = null, string expression = "")
        {
            if (propertyFields is null || propertyFields.Any())
            {
                propertyFields = _propertyFields;
            }
            if (expression == "")
            {
                expression = $"[{KeyField}] = @{KeyField}";
            }
            
            var queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("UPDATE [");
            queryStringBuilder.Append(TableName);
            queryStringBuilder.Append("] SET ");
            queryStringBuilder.AppendJoin(", ", MapFields(propertyFields));
            queryStringBuilder.Append(" WHERE ");
            queryStringBuilder.Append(expression);
            queryStringBuilder.Append(';');
            
            return new Query(queryStringBuilder);
        }

        public Query UpdateWhereQuery(ICollection<string>? propertyFields, ICondition<string>? conditions)
        {
            if (conditions is null || !conditions.Any())
            {
                return UpdateWhereQuery(propertyFields);
            }
            
            if (propertyFields is null || propertyFields.Any())
            {
                propertyFields = _propertyFields;
            }
            
            var queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("UPDATE [");
            queryStringBuilder.Append(TableName);
            queryStringBuilder.Append("] SET ");
            queryStringBuilder.AppendJoin(", ", MapFields(propertyFields));
            queryStringBuilder.Append(" WHERE ");
            queryStringBuilder.AppendJoin($" {ConditionConnectorString} ", MapFields(conditions));
            queryStringBuilder.Append(';');
            
            return new Query(queryStringBuilder);
        }
        
    }
}
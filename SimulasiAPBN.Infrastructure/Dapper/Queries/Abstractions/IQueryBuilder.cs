/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System.Collections.Generic;

namespace SimulasiAPBN.Infrastructure.Dapper.Queries.Abstractions
{
    public interface IQueryBuilder
    {
        string TableName { get; }
        string KeyField { get; }
        
        ConditionConnector ConditionConnector { get; set; }
        
        Query DeleteQuery();

        Query DeleteWhereQuery(string expression = "Id = @Id");

        Query DeleteWhereQuery(ICondition<string>? conditions);

        Query InsertQuery(ICollection<string>? propertyFields = null);

        SelectQuery SelectQuery(string projection = "*");

        SelectQuery SelectQuery(ICollection<string>? projectionFields);

        SelectQuery SelectWhereQuery(ICondition<string>? conditions = null, string projection = "*");

        SelectQuery SelectWhereQuery(ICondition<string>? conditions, ICollection<string>? projectionFields);

        Query UpdateWhereQuery(ICollection<string>? propertyFields = null, string expression = "");

        Query UpdateWhereQuery(ICollection<string>? propertyFields, ICondition<string>? conditions);
    }
}
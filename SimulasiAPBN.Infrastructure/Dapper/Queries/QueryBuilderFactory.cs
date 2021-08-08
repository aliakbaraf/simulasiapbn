/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Dapper.Contrib.Extensions;

namespace SimulasiAPBN.Infrastructure.Dapper.Queries
{
    public static class QueryBuilderFactory
    {
        public static QueryBuilder CreateQueryBuilder<TEntity>()
            where TEntity : class
        {
            var tableAttribute = (TableAttribute) Attribute
                .GetCustomAttribute(typeof(TEntity), typeof(TableAttribute));
            if (tableAttribute is null)
            {
                throw new InvalidConstraintException("Table Name was not set in the model.");
            }
            var tableName = tableAttribute.Name;

            var keyField = string.Empty;
            var propertyFields = new HashSet<string>();
            var propertyInfos = typeof(TEntity).GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var field = propertyInfo.Name;
                var keyAttribute = (KeyAttribute) propertyInfo.GetCustomAttribute(typeof(KeyAttribute));
                if (keyAttribute is not null)
                {
                    keyField = field;
                    propertyFields.Add(field);
                    continue;
                }
                var writeAttribute = (WriteAttribute) propertyInfo.GetCustomAttribute(typeof(WriteAttribute));
                if (writeAttribute is not null && !writeAttribute.Write)
                {
                    continue;
                }
                propertyFields.Add(field);
            }
            
            return new QueryBuilder(tableName, propertyFields, keyField);
        }
    }
}
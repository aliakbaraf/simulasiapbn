/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System;
using System.Data;
using System.Reflection;
using Dapper.Contrib.Extensions;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions;
using SimulasiAPBN.Infrastructure.Dapper.Queries;

namespace SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries
{
    public class ExecutableSelectQuery<TEntity> 
        : ExecutableQuery<TEntity>, IExecutableSelectQuery<TEntity> 
        where TEntity : class
    {
        public ExecutableSelectQuery(
            Query query, 
            IDbConnection dbConnection,
            IDbTransaction? dbTransaction) 
            : base(query, dbConnection, dbTransaction)
        {}
        
        public ExecutableSelectQuery(
            Query query, 
            object? param,
            IDbConnection dbConnection,
            IDbTransaction? dbTransaction) 
            : base(query, param, dbConnection, dbTransaction)
        {}
        
        private static bool HasProperty<TClass>(string propertyName) where TClass : class
            => typeof(TClass).GetProperty(propertyName) is not null;

        protected static string ProcessJoin<TLeftEntity, TRightEntity>(
            string propertyName, string referencePropertyName) 
            where TLeftEntity : class
            where TRightEntity : class
        {
            var referenceTableAttribute = (TableAttribute?) Attribute
                .GetCustomAttribute(typeof(TRightEntity), typeof (TableAttribute));
            if (referenceTableAttribute is null)
            {
                throw new InvalidConstraintException("Table Name was not set in the referenced model.");
            }
            var referenceTableName = referenceTableAttribute.Name;

            if (!HasProperty<TLeftEntity>(propertyName))
            {
                throw new ArgumentException(
                    $"Class { typeof(TLeftEntity).FullName } have no property named { propertyName }.",
                    propertyName);
            }
            
            if (!HasProperty<TRightEntity>(referencePropertyName))
            {
                throw new ArgumentException(
                    $"Class { typeof(TRightEntity).FullName } have no property named { propertyName }.",
                    propertyName);
            }

            return referenceTableName;
        }

        public SelectQuery SelectQuery => (SelectQuery) Query;
        
        public IExecutableJoinSelectQuery<TEntity, TJoinEntity> Join<TJoinEntity>(
            string propertyName, string referencePropertyName) 
            where TJoinEntity : class
        {
            var referenceTableName = ProcessJoin<TEntity, TJoinEntity>(propertyName, referencePropertyName);

            Query = SelectQuery.LeftJoin(referenceTableName, 
                propertyName, 
                referencePropertyName);

            return new ExecutableJoinSelectQuery<TEntity, TJoinEntity>(Query, Param, DbConnection, DbTransaction);
        }

        public IExecutableJoinSelectQuery<TEntity, TJoinEntity> Join<TJoinEntity>(
            PropertyInfo propertyInfo,
            PropertyInfo referencePropertyInfo)
            where TJoinEntity : class
            => Join<TJoinEntity>(propertyInfo.Name, referencePropertyInfo.Name);
    }
}
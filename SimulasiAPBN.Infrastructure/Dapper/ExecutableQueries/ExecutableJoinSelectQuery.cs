/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System.Data;
using System.Reflection;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions;
using SimulasiAPBN.Infrastructure.Dapper.Queries;

namespace SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries
{
    public class ExecutableJoinSelectQuery<TEntity, TJoinEntity> 
        : ExecutableSelectQuery<TEntity>, IExecutableJoinSelectQuery<TEntity, TJoinEntity> 
        where TEntity : class
        where TJoinEntity : class
    {
        public ExecutableJoinSelectQuery(
            Query query, 
            IDbConnection dbConnection, 
            IDbTransaction? dbTransaction) 
            : base(query, dbConnection, dbTransaction)
        {
        }

        public ExecutableJoinSelectQuery(
            Query query,
            object? param, 
            IDbConnection dbConnection, 
            IDbTransaction? dbTransaction) 
            : base(query, param, dbConnection, dbTransaction)
        {
        }

        public JoinSelectQuery JoinSelectQuery => (JoinSelectQuery) Query;
        
        public IExecutableJoinSelectQuery<TEntity, TThenJoinEntity> ThenJoin<TThenJoinEntity>(
            string propertyName, string referencePropertyName) 
            where TThenJoinEntity : class
        {
            var referenceTableName = ProcessJoin<TJoinEntity, TThenJoinEntity>(
                propertyName, referencePropertyName);

            Query = JoinSelectQuery.ThenLeftJoin(referenceTableName, 
                propertyName, 
                referencePropertyName);

            return new ExecutableJoinSelectQuery<TEntity, TThenJoinEntity>(Query, Param, DbConnection, DbTransaction);
        }

        public IExecutableJoinSelectQuery<TEntity, TThenJoinEntity> ThenJoin<TThenJoinEntity>(
            PropertyInfo propertyInfo, PropertyInfo referencePropertyInfo) 
            where TThenJoinEntity : class
            => ThenJoin<TThenJoinEntity>(propertyInfo.Name, referencePropertyInfo.Name);
    }
}
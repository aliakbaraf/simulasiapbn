/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Collections.Generic;
using System.Data;
using Dapper;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions;
using SimulasiAPBN.Infrastructure.Dapper.Queries;

namespace SimulasiAPBN.Infrastructure.Dapper.Extensions
{
    public static class DbConnectionExtension
    {
        public static IExecutableInsertQuery InsertOne<TEntity>(
            this IDbConnection dbConnection,
            TEntity entity,
            IDbTransaction dbTransaction)
            where TEntity : class
        {
            var queryBuilder = QueryBuilderFactory.CreateQueryBuilder<TEntity>();
            var query = queryBuilder.InsertQuery();
            
            return new ExecutableInsertQuery(query, entity, dbConnection, dbTransaction);
        }
        
        public static IExecutableInsertQuery InsertMany<TEntity>(
            this IDbConnection dbConnection,
            IEnumerable<TEntity> entities,
            IDbTransaction dbTransaction)
            where TEntity : class
        {
            var queryBuilder = QueryBuilderFactory.CreateQueryBuilder<TEntity>();
            var query = queryBuilder.InsertQuery();
            
            return new ExecutableInsertQuery(query, entities, dbConnection, dbTransaction);
        }
        
        public static IExecutableSelectQuery<TEntity> SelectAll<TEntity>(
            this IDbConnection dbConnection, 
            IDbTransaction dbTransaction)
            where TEntity : class
        {
            var queryBuilder = QueryBuilderFactory.CreateQueryBuilder<TEntity>();
            var query = queryBuilder.SelectQuery();
            
            return new ExecutableSelectQuery<TEntity>(query, dbConnection, dbTransaction);
        }
        
        public static IExecutableSelectQuery<TEntity> SelectById<TEntity>(
            this IDbConnection dbConnection, 
            dynamic id,
            IDbTransaction dbTransaction)
            where TEntity : class
        {
            var queryBuilder = QueryBuilderFactory.CreateQueryBuilder<TEntity>();
            var query = queryBuilder.SelectWhereQuery();
            var param = new DynamicParameters();
            param.Add($"@{queryBuilder.KeyField}", id);
            
            return new ExecutableSelectQuery<TEntity>(query, param, dbConnection, dbTransaction);
        }
    }
}
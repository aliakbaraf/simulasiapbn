/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using SimulasiAPBN.Common.Serializer;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions;
using SimulasiAPBN.Infrastructure.Dapper.Queries;

namespace SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries
{
    public class ExecutableQuery<TEntity> : IExecutableQuery<TEntity> where TEntity : class
    {
        private string _splitOn;
        private ILogger<IQueryLogger>? _logger;
        
        public ExecutableQuery(
            Query query, 
            IDbConnection dbConnection,
            IDbTransaction? dbTransaction)
        {
            DbConnection = dbConnection;
            DbTransaction = dbTransaction;
            Query = query;
            _splitOn = "Id";
        }
        
        public ExecutableQuery(
            Query query, 
            object? param,
            IDbConnection dbConnection,
            IDbTransaction? dbTransaction)
        {
            DbConnection = dbConnection;
            DbTransaction = dbTransaction;
            Param = param;
            Query = query;
            _splitOn = "Id";
        }

        private string PreProcessor(string queryString)
        {
            var param = "NULL";
            if (Param is DynamicParameters dynamicParameter)
            {
                var dictionary = dynamicParameter.ParameterNames
                    .ToDictionary(parameterName => 
                        parameterName, parameterName => 
                            dynamicParameter.Get<dynamic>(parameterName));
                param = Json.Serialize(dictionary);
            } 
            else if (Param is not null)
            {
                param = Json.Serialize(param);
            }
            
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger?.LogDebug($"Query: {queryString} Param: { param };");
            return queryString;
        }

        private static IEnumerable<TEntity> PostProcessor(IEnumerable<TEntity>? entities)
        {
            return (entities ?? new List<TEntity>()).Distinct();
        }
        
        public IDbConnection DbConnection { get; }
        public IDbTransaction? DbTransaction { get; }
        public object? Param { get; }
        public Query Query { get; protected set; }
        public string QueryString => Query.ToString();

        public IExecutableQuery<TEntity> SplitOn(string splitOn)
        {
            _splitOn = splitOn;
            return this;
        }

        public IEnumerable<TEntity> Execute()
        {
            var entities = DbConnection
                .Query<TEntity>(PreProcessor(QueryString), Param, DbTransaction);
            return PostProcessor(entities);
        }

        public async Task<IEnumerable<TEntity>> ExecuteAsync()
        {
            var entities = await DbConnection
                .QueryAsync<TEntity>(PreProcessor(QueryString), Param, DbTransaction);
            return PostProcessor(entities);
        }

        public IEnumerable<TEntity> Execute<TFirst>(
            Func<TEntity, TFirst, TEntity> map)
        {
            var entities = DbConnection.Query(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(entities);
        }

        public async Task<IEnumerable<TEntity>> ExecuteAsync<TFirst>(
            Func<TEntity, TFirst, TEntity> map)
        {
            var entities = await DbConnection.QueryAsync(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(entities);
        }

        public IEnumerable<TEntity> Execute<TFirst, TSecond>(
            Func<TEntity, TFirst, TSecond, TEntity> map)
        {
            var entities = DbConnection.Query(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(entities);
        }

        public async Task<IEnumerable<TEntity>> ExecuteAsync<TFirst, TSecond>(
            Func<TEntity, TFirst, TSecond, TEntity> map)
        {
            var entities = await DbConnection.QueryAsync(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(entities);
        }

        public IEnumerable<TEntity> Execute<TFirst, TSecond, TThird>(
            Func<TEntity, TFirst, TSecond, TThird, TEntity> map)
        {
            var entities = DbConnection.Query(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(entities);
        }

        public async Task<IEnumerable<TEntity>> ExecuteAsync<TFirst, TSecond, TThird>(
            Func<TEntity, TFirst, TSecond, TThird, TEntity> map)
        {
            var entities = await DbConnection.QueryAsync(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(entities);
        }

        public IEnumerable<TEntity> Execute<TFirst, TSecond, TThird, TFourth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TEntity> map)
        {
            var entities = DbConnection.Query(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(entities);
        }

        public async Task<IEnumerable<TEntity>> ExecuteAsync<TFirst, TSecond, TThird, TFourth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TEntity> map)
        {
            var entities = await DbConnection.QueryAsync(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(entities);
        }

        public IEnumerable<TEntity> Execute<TFirst, TSecond, TThird, TFourth, TFifth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TFifth, TEntity> map)
        {
            var entities = DbConnection.Query(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(entities);
        }

        public async Task<IEnumerable<TEntity>> ExecuteAsync<TFirst, TSecond, TThird, TFourth, TFifth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TFifth, TEntity> map)
        {
            var entities = await DbConnection.QueryAsync(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(entities);
        }

        public TEntity? ExecuteSingle()
        {
            var result = DbConnection
                .Query<TEntity>(PreProcessor(QueryString), Param, DbTransaction);
            return PostProcessor(result).FirstOrDefault();
        }

        public async Task<TEntity?> ExecuteSingleAsync()
        {
            var result = await DbConnection
                .QueryAsync<TEntity>(PreProcessor(QueryString), Param, DbTransaction);
            return PostProcessor(result).FirstOrDefault();
        }

        public TEntity? ExecuteSingle<TFirst>(
            Func<TEntity, TFirst, TEntity> map)
        {
            var result = DbConnection.Query(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(result).FirstOrDefault();
        }

        public async Task<TEntity?> ExecuteSingleAsync<TFirst>(
            Func<TEntity, TFirst, TEntity> map)
        {
            var result = await DbConnection.QueryAsync(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(result).FirstOrDefault();
        }

        public TEntity? ExecuteSingle<TFirst, TSecond>(
            Func<TEntity, TFirst, TSecond, TEntity> map)
        {
            var result = DbConnection.Query(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(result).FirstOrDefault();
        }

        public async Task<TEntity?> ExecuteSingleAsync<TFirst, TSecond>(
            Func<TEntity, TFirst, TSecond, TEntity> map)
        {
            var result = await DbConnection.QueryAsync(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(result).FirstOrDefault();
        }

        public TEntity? ExecuteSingle<TFirst, TSecond, TThird>(
            Func<TEntity, TFirst, TSecond, TThird, TEntity> map)
        {
            var result = DbConnection.Query(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(result).FirstOrDefault();
        }

        public async Task<TEntity?> ExecuteSingleAsync<TFirst, TSecond, TThird>(
            Func<TEntity, TFirst, TSecond, TThird, TEntity> map)
        {
            var result = await DbConnection.QueryAsync(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(result).FirstOrDefault();
        }

        public TEntity? ExecuteSingle<TFirst, TSecond, TThird, TFourth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TEntity> map)
        {
            var result = DbConnection.Query(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(result).FirstOrDefault();
        }

        public async Task<TEntity?> ExecuteSingleAsync<TFirst, TSecond, TThird, TFourth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TEntity> map)
        {
            var result = await DbConnection.QueryAsync(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(result).FirstOrDefault();
        }

        public TEntity? ExecuteSingle<TFirst, TSecond, TThird, TFourth, TFifth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TFifth, TEntity> map)
        {
            var result = DbConnection.Query(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(result).FirstOrDefault();
        }

        public async Task<TEntity?> ExecuteSingleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TFifth, TEntity> map)
        {
            var result = await DbConnection.QueryAsync(PreProcessor(QueryString),
                param: Param,
                transaction: DbTransaction,
                map: map, 
                splitOn: _splitOn);
            return PostProcessor(result).FirstOrDefault();
        }

        public void UseLogger(ILogger<IQueryLogger> logger)
        {
            _logger = logger;
        }

        public void UseLogger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<IQueryLogger>();
        }
    }
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
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
    public class ExecutableInsertQuery : IExecutableInsertQuery
    {
        private ILogger<IQueryLogger>? _logger;
        
        public ExecutableInsertQuery(
            Query query, 
            object param,
            IDbConnection dbConnection,
            IDbTransaction? dbTransaction)
        {
            DbConnection = dbConnection;
            DbTransaction = dbTransaction;
            Param = param;
            Query = query;
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
        
        public IDbConnection DbConnection { get; }
        public IDbTransaction? DbTransaction { get; }
        public object? Param { get; }
        public Query Query { get; }
        public string QueryString => Query.ToString();
        
        public int Execute()
        {
            return DbConnection.Execute(PreProcessor(QueryString), Param, DbTransaction);
        }

        public Task<int> ExecuteAsync()
        {
            return DbConnection.ExecuteAsync(PreProcessor(QueryString), Param, DbTransaction);
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
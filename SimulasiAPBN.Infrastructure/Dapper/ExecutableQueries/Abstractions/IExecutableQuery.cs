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
using System.Threading.Tasks;
using SimulasiAPBN.Infrastructure.Dapper.Queries;

namespace SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions
{
    public interface IExecutableQuery<TEntity> : IQueryLogger where TEntity : class
    {
        IDbConnection DbConnection { get; }
        IDbTransaction? DbTransaction { get; }
        object? Param { get; }
        Query Query { get; }
        string QueryString { get; }
        
        IExecutableQuery<TEntity> SplitOn(string splitOn);

        IEnumerable<TEntity> Execute();
        Task<IEnumerable<TEntity>> ExecuteAsync();
        
        IEnumerable<TEntity> Execute<TFirst>(Func<TEntity, TFirst, TEntity> map);
        Task<IEnumerable<TEntity>> ExecuteAsync<TFirst>(Func<TEntity, TFirst, TEntity> map);
        
        IEnumerable<TEntity> Execute<TFirst, TSecond>(
            Func<TEntity, TFirst, TSecond, TEntity> map);
        Task<IEnumerable<TEntity>> ExecuteAsync<TFirst, TSecond>(
            Func<TEntity, TFirst, TSecond, TEntity> map);
        
        IEnumerable<TEntity> Execute<TFirst, TSecond, TThird>(
            Func<TEntity, TFirst, TSecond, TThird, TEntity> map);
        Task<IEnumerable<TEntity>> ExecuteAsync<TFirst, TSecond, TThird>(
            Func<TEntity, TFirst, TSecond, TThird, TEntity> map);
        
        IEnumerable<TEntity> Execute<TFirst, TSecond, TThird, TFourth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TEntity> map);
        Task<IEnumerable<TEntity>> ExecuteAsync<TFirst, TSecond, TThird, TFourth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TEntity> map);
        
        IEnumerable<TEntity> Execute<TFirst, TSecond, TThird, TFourth, TFifth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TFifth, TEntity> map);
        Task<IEnumerable<TEntity>> ExecuteAsync<TFirst, TSecond, TThird, TFourth, TFifth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TFifth, TEntity> map);
        
        TEntity? ExecuteSingle();
        Task<TEntity?> ExecuteSingleAsync();
        
        TEntity? ExecuteSingle<TFirst>(Func<TEntity, TFirst, TEntity> map);
        Task<TEntity?> ExecuteSingleAsync<TFirst>(Func<TEntity, TFirst, TEntity> map);
        
        TEntity? ExecuteSingle<TFirst, TSecond>(
            Func<TEntity, TFirst, TSecond, TEntity> map);
        Task<TEntity?> ExecuteSingleAsync<TFirst, TSecond>(
            Func<TEntity, TFirst, TSecond, TEntity> map);
        
        TEntity? ExecuteSingle<TFirst, TSecond, TThird>(
            Func<TEntity, TFirst, TSecond, TThird, TEntity> map);
        Task<TEntity?> ExecuteSingleAsync<TFirst, TSecond, TThird>(
            Func<TEntity, TFirst, TSecond, TThird, TEntity> map);
        
        TEntity? ExecuteSingle<TFirst, TSecond, TThird, TFourth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TEntity> map);
        Task<TEntity?> ExecuteSingleAsync<TFirst, TSecond, TThird, TFourth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TEntity> map);
        
        TEntity? ExecuteSingle<TFirst, TSecond, TThird, TFourth, TFifth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TFifth, TEntity> map);
        Task<TEntity?> ExecuteSingleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(
            Func<TEntity, TFirst, TSecond, TThird, TFourth, TFifth, TEntity> map);
    }
}
/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Reflection;
using SimulasiAPBN.Infrastructure.Dapper.Queries;

namespace SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions
{
    // ReSharper disable once UnusedTypeParameter
    public interface IExecutableJoinSelectQuery<TEntity, TJoinEntity> 
        : IExecutableSelectQuery<TEntity>
        where TEntity : class
        where TJoinEntity : class 
    {
        JoinSelectQuery JoinSelectQuery { get; }
        
        IExecutableJoinSelectQuery<TEntity, TThenJoinEntity> ThenJoin<TThenJoinEntity>(
            string propertyName, string referencePropertyName) 
            where TThenJoinEntity : class;
        
        IExecutableJoinSelectQuery<TEntity, TThenJoinEntity> ThenJoin<TThenJoinEntity>(
            PropertyInfo propertyInfo, PropertyInfo referencePropertyInfo) 
            where TThenJoinEntity : class;
    }
}
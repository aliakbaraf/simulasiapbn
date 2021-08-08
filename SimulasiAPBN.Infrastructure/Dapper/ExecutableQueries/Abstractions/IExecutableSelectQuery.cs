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
    public interface IExecutableSelectQuery<TEntity> : IExecutableQuery<TEntity> where TEntity : class
    {
        SelectQuery SelectQuery { get; }

        IExecutableJoinSelectQuery<TEntity, TJoinEntity> Join<TJoinEntity>(
            string propertyName, string referencePropertyName) 
            where TJoinEntity : class;
        
        IExecutableJoinSelectQuery<TEntity, TJoinEntity> Join<TJoinEntity>(
            PropertyInfo propertyInfo, PropertyInfo referencePropertyInfo) 
            where TJoinEntity : class;
    }
}
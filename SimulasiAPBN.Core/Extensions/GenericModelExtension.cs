/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Core.Extensions
{
    public static class GenericModelExtension
    {
        public static TEntity MarkCreated<TEntity>(this TEntity entity) 
            where TEntity : GenericModel
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }
            entity.CreatedAt = DateTimeOffset.Now;
            return entity;
        }

        public static TEntity MarkUpdated<TEntity>(this TEntity entity) 
            where TEntity : GenericModel
        {
            entity.UpdatedAt = DateTimeOffset.Now;
            return entity;
        }

        public static TEntity MarkDeleted<TEntity>(this TEntity entity) 
            where TEntity : GenericModel
        {
            entity.DeletedAt = DateTimeOffset.Now;
            return entity;
        }
    }
}
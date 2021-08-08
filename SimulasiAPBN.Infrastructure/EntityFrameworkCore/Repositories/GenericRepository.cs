/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Infrastructure.EntityFrameworkCore.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity, RepositoryOptions> 
        where TEntity : GenericModel, new()
    {
        
        #region Constructor
        protected GenericRepository(RepositoryOptions options)
        {
            Options = options;
        }
        #endregion

        #region Helper Methods
        protected IEnumerable<TEntity> ProcessQuery(IQueryable<TEntity> query)
        {
            query = Options.IgnoreSoftDeleteProperty ? query.IgnoreQueryFilters() : query;
            return query.AsNoTracking().ToList();
        }
        protected async Task<IEnumerable<TEntity>> ProcessQueryAsync(IQueryable<TEntity> query)
        {
            query = Options.IgnoreSoftDeleteProperty ? query.IgnoreQueryFilters() : query;
            return await query.AsNoTracking().ToListAsync();
        }
        #endregion

        #region Helper Properties
        protected virtual DbSet<TEntity> EntityDbSet => Options.DbContext.Set<TEntity>();
        protected virtual IQueryable<TEntity> EntityQuery => EntityDbSet.AsNoTracking();
        #endregion

        #region IGenericRepository Implementation
        public RepositoryOptions Options { get; }

        public virtual int Add(TEntity entity)
        {
            var entry = EntityDbSet.Add(entity);
            return entry.State == EntityState.Added ? 1 : 0;
        }

        public virtual async Task<int> AddAsync(TEntity entity)
        {
            var entry = await EntityDbSet.AddAsync(entity);
            return entry.State == EntityState.Added ? 1 : 0;
        }

        public virtual int AddRange(IEnumerable<TEntity> entities)
        {
            var enumerable = entities.ToList();
            EntityDbSet.AddRange(enumerable);
            return enumerable.Count();
        }

        public virtual async Task<int> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            var enumerable = entities.ToList();
            await EntityDbSet.AddRangeAsync(enumerable);
            return enumerable.Count();
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
        {
            return ProcessQuery(EntityQuery.Where(expression));
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression)
        {
            return ProcessQueryAsync(EntityQuery.Where(expression));
        }

        public virtual TEntity? FindOne(Expression<Func<TEntity, bool>> expression)
        {
            return Find(expression).FirstOrDefault();
        }

        public virtual async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> expression)
        {
            return (await FindAsync(expression)).FirstOrDefault();
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return ProcessQuery(EntityQuery);
        }

        public virtual Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return ProcessQueryAsync(EntityQuery);
        }

        public virtual TEntity? GetById(Guid id)
        {
            return FindOne(entity => entity.Id == id);
        }

        public virtual Task<TEntity?> GetByIdAsync(Guid id)
        {
            return FindOneAsync(entity => entity.Id == id);
        }

        public virtual bool Modify(TEntity entity)
        {
            var entry = EntityDbSet.Update(entity);
            return entry.State == EntityState.Modified;
        }

        public virtual Task<bool> ModifyAsync(TEntity entity)
        {
            return Task.Run(() => Modify(entity));
        }

        public virtual bool ModifyRange(IEnumerable<TEntity> entities)
        {
            EntityDbSet.UpdateRange(entities);
            return true;
        }

        public virtual Task<bool> ModifyRangeAsync(IEnumerable<TEntity> entities)
        {
            return Task.Run(() => ModifyRange(entities));
        }

        public virtual bool Remove(TEntity entity)
        {
            var entry = EntityDbSet.Remove(entity);
            return entry.State == EntityState.Deleted;
        }

        public virtual Task<bool> RemoveAsync(TEntity entity)
        {
            return Task.Run(() => Remove(entity));
        }

        public virtual bool RemoveById(Guid id)
        {
            var entity = GetById(id);
            if (entity is null) return false;
            
            var entry = EntityDbSet.Remove(entity);
            return entry.State == EntityState.Deleted;
        }

        public virtual Task<bool> RemoveByIdAsync(Guid id)
        {
            return Task.Run(() => RemoveById(id));
        }

        public virtual bool RemoveRange(IEnumerable<TEntity> entities)
        {
            EntityDbSet.RemoveRange(entities);
            return true;
        }

        public virtual Task<bool> RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            return Task.Run(() => RemoveRange(entities));
        }
        #endregion
    }
}

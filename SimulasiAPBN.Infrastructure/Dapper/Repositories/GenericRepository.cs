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
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Extensions;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries;
using SimulasiAPBN.Infrastructure.Dapper.ExecutableQueries.Abstractions;
using SimulasiAPBN.Infrastructure.Dapper.Extensions;

namespace SimulasiAPBN.Infrastructure.Dapper.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity, RepositoryOptions> 
        where TEntity : GenericModel
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger _logger;

        #region Constructor
        protected GenericRepository(RepositoryOptions options)
        {
            Options = options;
            _logger = Options.LoggerFactory.CreateLogger(typeof(IExecutableQuery<>));
        }
        #endregion

        #region Helper Methods
        protected virtual IExecutableInsertQuery InsertQueryProcessor(
            IExecutableInsertQuery executableInsertQuery)
        {
            return QueryProcessor(executableInsertQuery);
        }
        
        protected virtual IExecutableSelectQuery<TEntity> SelectQueryProcessor(
            IExecutableSelectQuery<TEntity> executableSelectQuery)
        {
            if (Options.IgnoreSoftDeleteProperty)
            {
                return QueryProcessor(executableSelectQuery);
            }

            var selectQuery = executableSelectQuery.SelectQuery.Where("DeletedAt", "IS", "NULL");
            foreach (var joinQuery in selectQuery.JoinQueries)
            {
                selectQuery = selectQuery.Where(joinQuery.Alias, "DeletedAt", "IS", "NULL");
            }
            executableSelectQuery = new ExecutableSelectQuery<TEntity>(
                selectQuery, executableSelectQuery.Param, executableSelectQuery.DbConnection, executableSelectQuery.DbTransaction);
            return QueryProcessor(executableSelectQuery);
        }
        
        protected virtual TExecutableQuery QueryProcessor<TExecutableQuery>(
            TExecutableQuery executableQuery)
            where TExecutableQuery : class
        {
            (executableQuery as IQueryLogger)?.UseLogger(Options.LoggerFactory);
            return executableQuery;
        }

        protected void UseMapping(dynamic entityMapper, string splitOn = "Id")
        {
            EntityMapper = entityMapper;
            SplitOn = splitOn;
        }
        #endregion

        #region Helper Properties
        private IDbConnection DbConnection => Options.DbConnection;
        private IDbTransaction? DbTransaction => Options.DbTransaction;
        private dynamic? EntityMapper { get; set; }

        private string? SplitOn { get; set; }
        #endregion
        
        #region IGenericRepository Implementation
        public RepositoryOptions Options { get; }
        
        public virtual int Add(TEntity entity)
        {
            entity.MarkCreated();
            var query = DbConnection.InsertOne(entity, DbTransaction);
            return InsertQueryProcessor(query).Execute();
        }

        public virtual Task<int> AddAsync(TEntity entity)
        {
            entity.MarkCreated();
            var query = DbConnection.InsertOne(entity, DbTransaction);
            return InsertQueryProcessor(query).ExecuteAsync();
        }

        public virtual int AddRange(IEnumerable<TEntity> entities)
        {
            entities = entities.Select(entity => entity.MarkCreated());
            var query = DbConnection.InsertMany(entities, DbTransaction);
            return InsertQueryProcessor(query).Execute();
        }

        public virtual Task<int> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            entities = entities.Select(entity => entity.MarkCreated());
            var query = DbConnection.InsertMany(entities, DbTransaction);
            return InsertQueryProcessor(query).ExecuteAsync();
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
        {
            var entities = GetAll();
            return entities.Where(expression.Compile());
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entities = await GetAllAsync();
            return entities.Where(expression.Compile());
        }

        public virtual TEntity? FindOne(Expression<Func<TEntity, bool>> expression)
        {
            var entities = GetAll();
            return entities.FirstOrDefault(expression.Compile());
        }

        public virtual async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entities = await GetAllAsync();
            return entities.FirstOrDefault(expression.Compile());
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            var query = SelectQueryProcessor(DbConnection.SelectAll<TEntity>(DbTransaction));
            if (SplitOn is not null)
            {
                query = (IExecutableSelectQuery<TEntity>) query.SplitOn(SplitOn);
            }
            var entities = EntityMapper is not null 
                ? query.Execute(EntityMapper()) 
                : query.Execute();
            return entities;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var query = SelectQueryProcessor(DbConnection.SelectAll<TEntity>(DbTransaction));
            if (SplitOn is not null)
            {
                query = (IExecutableSelectQuery<TEntity>) query.SplitOn(SplitOn);
            }
            var entities = EntityMapper is not null 
                ? await query.ExecuteAsync(EntityMapper()) 
                : await query.ExecuteAsync();
            return entities;
        }

        // ReSharper disable once ReturnTypeCanBeNotNullable
        public virtual TEntity? GetById(Guid id)
        {
            var query = SelectQueryProcessor(DbConnection.SelectById<TEntity>(id, DbTransaction));
            if (SplitOn is not null)
            {
                query = (IExecutableSelectQuery<TEntity>) query.SplitOn(SplitOn);
            }
            var entity = EntityMapper is not null 
                ? query.ExecuteSingle(EntityMapper()) 
                : query.ExecuteSingle();
            return entity;
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            var query = SelectQueryProcessor(DbConnection.SelectById<TEntity>(id, DbTransaction));
            if (SplitOn is not null)
            {
                query = (IExecutableSelectQuery<TEntity>) query.SplitOn(SplitOn);
            }
            var entity = EntityMapper is not null 
                ? await query.ExecuteSingleAsync(EntityMapper()) 
                : await query.ExecuteSingleAsync();
            return entity;
        }

        public virtual bool Modify(TEntity entity)
        {
            entity = entity.MarkUpdated();
            return DbConnection.Update(entity, DbTransaction);
        }

        public virtual Task<bool> ModifyAsync(TEntity entity)
        {
            entity = entity.MarkUpdated();
            return DbConnection.UpdateAsync(entity, DbTransaction);
        }

        public virtual bool ModifyRange(IEnumerable<TEntity> entities)
        {
            entities = entities.Select(entity => entity.MarkUpdated());
            return DbConnection.Update(entities, DbTransaction);
        }

        public virtual Task<bool> ModifyRangeAsync(IEnumerable<TEntity> entities)
        {
            entities = entities.Select(entity => entity.MarkUpdated());
            return DbConnection.UpdateAsync(entities, DbTransaction);
        }

        public virtual bool Remove(TEntity entity)
        {
            if (Options.IgnoreSoftDeleteProperty)
            {
                return DbConnection.Delete(entity, DbTransaction);
            }

            entity = entity.MarkDeleted();
            return DbConnection.Update(entity, DbTransaction);
        }

        public virtual Task<bool> RemoveAsync(TEntity entity)
        {
            if (Options.IgnoreSoftDeleteProperty)
            {
                return DbConnection.DeleteAsync(entity, DbTransaction);
            }

            entity = entity.MarkDeleted();
            return DbConnection.UpdateAsync(entity, DbTransaction);
        }

        public virtual bool RemoveById(Guid id)
        {
            var entity = GetById(id);

            return entity is not null && Remove(entity);
        }

        public virtual async Task<bool> RemoveByIdAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);

            return entity is not null && await RemoveAsync(entity);
        }

        public virtual bool RemoveRange(IEnumerable<TEntity> entities)
        {
            if (Options.IgnoreSoftDeleteProperty)
            {
                return DbConnection.Delete(entities, DbTransaction);
            }

            entities = entities.Select(entity => entity.MarkDeleted());
            return DbConnection.Update(entities, DbTransaction);
        }

        public virtual Task<bool> RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            if (Options.IgnoreSoftDeleteProperty)
            {
                return DbConnection.DeleteAsync(entities, DbTransaction);
            }

            entities = entities.Select(entity => entity.MarkDeleted());
            return DbConnection.UpdateAsync(entities, DbTransaction);
        }
        #endregion
    }
}

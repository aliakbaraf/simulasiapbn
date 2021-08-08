/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface IGenericRepository<TEntity> : IGenericRepository<TEntity, IRepositoryOptions>
        where TEntity : GenericModel {}

    public interface IGenericRepository<TEntity, out TRepositoryOptions> 
        where TEntity : GenericModel 
        where TRepositoryOptions : IRepositoryOptions
    {

        TRepositoryOptions Options { get; }

        int Add(TEntity entity);
        Task<int> AddAsync(TEntity entity);

        int AddRange(IEnumerable<TEntity> entities);
        Task<int> AddRangeAsync(IEnumerable<TEntity> entities);
        
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression);
        
        TEntity? FindOne(Expression<Func<TEntity, bool>> expression);
        Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> expression);

        IEnumerable<TEntity> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync();

        TEntity? GetById(Guid id);
        Task<TEntity?> GetByIdAsync(Guid id);

        bool Modify(TEntity entity);
        Task<bool> ModifyAsync(TEntity entity);
        
        bool ModifyRange(IEnumerable<TEntity> entities);
        Task<bool> ModifyRangeAsync(IEnumerable<TEntity> entities);

        bool Remove(TEntity entity);
        Task<bool> RemoveAsync(TEntity entity);

        bool RemoveById(Guid id);
        Task<bool> RemoveByIdAsync(Guid id);

        bool RemoveRange(IEnumerable<TEntity> entities);
        Task<bool> RemoveRangeAsync(IEnumerable<TEntity> entities);

    }

}

using System.Linq.Expressions;

namespace NetUtils.Repository
{
    public interface IRepositorySlim<TEntity, TId> where TEntity : class
    {
        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        ValueTask<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
        ValueTask<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);
        ValueTask<TEntity?> DeleteAsync(TId id, CancellationToken ct = default);
        ValueTask UpdateAsync(TEntity entity);
    }
}

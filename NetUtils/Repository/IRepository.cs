using System.Linq.Expressions;

namespace NetUtils.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<bool> ExistAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, CancellationToken ct = default);
        ValueTask<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
        ValueTask<T> AddAsync(T entity, CancellationToken ct = default);
        ValueTask<T?> DeleteAsync(int id, CancellationToken ct = default);
        ValueTask UpdateAsync(T entity);
        Task<int> SaveChangeAsync(CancellationToken ct = default);

    }
}

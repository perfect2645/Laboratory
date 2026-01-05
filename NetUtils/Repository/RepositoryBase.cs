using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace NetUtils.Repository
{
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected DbContext DbContext { get; init; }
        protected DbSet<T> DbSet { get; init; }

        public RepositoryBase(DbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return await DbSet.AnyAsync(predicate, ct);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        {
            return predicate == null
                ? await DbSet.AsNoTracking().ToListAsync(ct)
                : await DbSet.AsNoTracking().Where(predicate).ToListAsync(ct);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, CancellationToken ct = default)
        {
            var query = asNoTracking ? DbSet.AsNoTracking() : DbSet;
            return await query.FirstOrDefaultAsync(predicate, ct);
        }

        public async ValueTask<T?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await DbSet.FindAsync(id, ct);
        }

        public async ValueTask<T> AddAsync(T entity, CancellationToken ct = default)
        {
            var addedEntry = await DbSet.AddAsync(entity, ct);
            return addedEntry.Entity;
        }

        public async ValueTask<T?> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await GetByIdAsync(id, ct);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }

            return entity;
        }

        public async ValueTask UpdateAsync(T entity)
        {
            DbSet.Update(entity);
            await ValueTask.CompletedTask;
        }

        public async Task<int> SaveChangeAsync(CancellationToken ct = default)
        {
            return await DbContext.SaveChangesAsync(ct);
        }
    }
}

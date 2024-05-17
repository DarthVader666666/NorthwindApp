using Northwind.Data;

namespace Northwind.Bll.Interfaces
{
    public abstract class RepositoryBase<TEntity, TDbContext> : IRepository<TEntity>
        where TEntity : class 
        where TDbContext : NorthwindDbContext
    {
        protected RepositoryBase(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        protected NorthwindDbContext DbContext { get; }

        public virtual async Task<TEntity?> CreateAsync(TEntity item)
        {
            var entity = (await DbContext.AddAsync(item)).Entity;

            return await SaveAsync(entity);
        }

        public virtual async Task<TEntity?> DeleteAsync(object? id)
        {
            var item = await DbContext.FindAsync<TEntity>(id);

            if (item == null)
            {
                return item;
            }

            var entity = DbContext.Remove(item).Entity;

            return await SaveAsync(entity);
        }

        public virtual async Task<int> DeleteSeveralAsync(int[] ids)
        {
            int count = 0;

            foreach (var id in ids!)
            {
                await DeleteAsync(id);
                count++;
            }

            return count;
        }

        public virtual async Task<int> DeleteSeveralAsync(string[] ids)
        {
            int count = 0;

            foreach (var id in ids!)
            {
                await DeleteAsync(id);
                count++;
            }

            return count;
        }

        public virtual async Task<TEntity?> GetAsync(object? id)
        {
            return await DbContext.FindAsync<TEntity>(id);
        }

        public Task<IEnumerable<TEntity?>> GetListAsync()
        {
            return Task.Run(() => DbContext.Set<TEntity?>().AsEnumerable());
        }

        public virtual Task<IEnumerable<TEntity?>> GetRangeAsync(int[] ids)
        {
            return Task.Run(() =>
            {
                return GetEntities();

                IEnumerable<TEntity?> GetEntities()
                {
                    foreach (var id in ids)
                    {
                        yield return GetAsync(id).Result;
                    }
                }                
            });            
        }

        public virtual Task<IEnumerable<TEntity?>> GetRangeAsync(string[] ids)
        {
            return Task.Run(() =>
            {
                return GetEntities();

                IEnumerable<TEntity?> GetEntities()
                {
                    foreach (var id in ids)
                    {
                        yield return GetAsync(id).Result;
                    }
                }
            });
        }

        public async Task<TEntity?> UpdateAsync(TEntity item)
        {
            DbContext.Update(item);

            return await SaveAsync(item);
        }

        public async Task<bool> ExistsAsync(TEntity item)
        {
            var result = await DbContext.FindAsync<TEntity>(item);
            return result != null;
        }

        protected async Task<TEntity?> SaveAsync(TEntity? item)
        {
            return await DbContext.SaveChangesAsync() > 0 ? item : null; 
        }

        public virtual Task<IEnumerable<TEntity?>> GetListForAsync(int? foreignKey)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<TEntity?>> GetListForAsync(string? foreignKey)
        {
            throw new NotImplementedException();
        }
    }
}

using Northwind.Data;

namespace Northwind.Bll.Interfaces
{
    public abstract class RepositoryBase<TEntity, TDbContext> : IGuestRepository<TEntity>
        where TEntity : class 
        where TDbContext : NorthwindDbContext
    {
        protected RepositoryBase(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public NorthwindDbContext DbContext { get; }

        public async Task<TEntity?> Create(TEntity item)
        {
            var entity = (await DbContext.AddAsync(item)).Entity;

            return await SaveAsync(entity);
        }

        public async Task<TEntity?> Delete(int? id)
        {
            var item = await DbContext.FindAsync<TEntity>(id);

            if (item == null)
            {
                return item;
            }

            var entity = DbContext.Remove(item).Entity;

            return await SaveAsync(entity);
        }

        public async Task<int> DeleteSeveral(int[]? ids)
        {
            foreach (var id in ids!)
            {
                var item = await DbContext.FindAsync<TEntity>(id);

                if (item != null)
                {
                    DbContext.Remove(item);
                }
            }

            return await DbContext.SaveChangesAsync();
        }

        public async Task<TEntity?> Get(int? id)
        {
            return await DbContext.FindAsync<TEntity>(id);
        }

        public IEnumerable<TEntity> GetList()
        {
            return DbContext.Set<TEntity>().AsEnumerable();
        }

        public async Task<TEntity?> Update(TEntity item)
        {
            DbContext.Update(item);

            return await SaveAsync(item);
        }

        protected async Task<TEntity?> SaveAsync(TEntity? item)
        {
            return await DbContext.SaveChangesAsync() > 0 ? item : null; 
        }
    }
}

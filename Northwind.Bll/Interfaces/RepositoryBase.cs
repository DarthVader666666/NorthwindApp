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

        public NorthwindDbContext DbContext { get; }

        public async Task<TEntity?> CreateAsync(TEntity item)
        {
            var entity = (await DbContext.AddAsync(item)).Entity;

            return await SaveAsync(entity);
        }

        public virtual async Task<TEntity?> DeleteAsync(int? id)
        {
            var item = await DbContext.FindAsync<TEntity>(id);

            if (item == null)
            {
                return item;
            }

            var entity = DbContext.Remove(item).Entity;

            return await SaveAsync(entity);
        }

        public virtual async Task<int> DeleteSeveralAsync(int[]? ids)
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

        public virtual async Task<TEntity?> GetAsync(int? id)
        {
            return await DbContext.FindAsync<TEntity>(id);
        }

        public IEnumerable<TEntity> GetList()
        {
            return DbContext.Set<TEntity>().AsEnumerable();
        }

        public virtual IEnumerable<TEntity?> GetListFor(int fkId)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity?> UpdateAsync(TEntity item)
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

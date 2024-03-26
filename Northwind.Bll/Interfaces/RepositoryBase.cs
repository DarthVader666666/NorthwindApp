using Microsoft.EntityFrameworkCore;

namespace Northwind.Bll.Interfaces
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;

        protected RepositoryBase(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TEntity?> Create(TEntity item)
        {
            var entity = (await _dbContext.AddAsync(item)).Entity;

            return await SaveAsync(entity);
        }

        public async Task<TEntity?> Delete(int? id)
        {
            var item = await _dbContext.FindAsync<TEntity>(id);

            if (item == null)
            {
                return item;
            }

            var entity = _dbContext.Remove(item).Entity;

            return await SaveAsync(entity);
        }

        public virtual async Task<TEntity?> Get(int? id)
        {
            return await _dbContext.FindAsync<TEntity>(id);
        }

        public virtual IEnumerable<TEntity> GetList()
        {
            return _dbContext.Set<TEntity>().AsEnumerable();
        }

        public async Task<TEntity?> Update(TEntity item)
        {
            _dbContext.Update(item);

            return await SaveAsync(item);
        }

        protected async Task<TEntity?> SaveAsync(TEntity? item)
        {
            return await _dbContext.SaveChangesAsync() > 0 ? item : null; 
        }
    }
}

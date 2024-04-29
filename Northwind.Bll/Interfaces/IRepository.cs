namespace Northwind.Bll.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity?>> GetListAsync();
        Task<TEntity?> GetAsync(object id);
        Task<IEnumerable<TEntity?>> GetRangeAsync(params object[] ids);
        Task<TEntity?> CreateAsync(TEntity item);
        Task<TEntity?> UpdateAsync(TEntity item);
        Task<TEntity?> DeleteAsync(object id);
        Task<int> DeleteSeveralAsync(params object[] ids);
        Task<IEnumerable<TEntity?>> GetListForAsync(int fkId);
    }
}

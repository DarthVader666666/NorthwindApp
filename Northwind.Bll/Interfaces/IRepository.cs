namespace Northwind.Bll.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity?>> GetListAsync();
        Task<TEntity?> GetAsync(int? id);
        Task<IEnumerable<TEntity?>> GetRangeAsync(params int?[] ids);
        Task<TEntity?> CreateAsync(TEntity item);
        Task<TEntity?> UpdateAsync(TEntity item);
        Task<TEntity?> DeleteAsync(int? id);
        Task<int> DeleteSeveralAsync(int?[] ids);
        Task<IEnumerable<TEntity?>> GetListForAsync(int fkId);
    }
}

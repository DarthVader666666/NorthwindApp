namespace Northwind.Bll.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity?>> GetListAsync();
        Task<TEntity?> GetAsync(object? id);
        Task<IEnumerable<TEntity?>> GetRangeAsync(int[] ids);
        Task<IEnumerable<TEntity?>> GetRangeAsync(string[] ids);
        Task<TEntity?> CreateAsync(TEntity item);
        Task<TEntity?> UpdateAsync(TEntity item);
        Task<TEntity?> DeleteAsync(object? id);
        Task<int> DeleteSeveralAsync(int[] ids);
        Task<int> DeleteSeveralAsync(string[] ids);
        Task<IEnumerable<TEntity?>> GetListForAsync(int? fkId);
        Task<IEnumerable<TEntity?>> GetListForAsync(string fkId);
        Task<bool> ExistsAsync(TEntity item);
    }
}

namespace Northwind.Bll.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity?> GetList();
        Task<TEntity?> GetAsync(int? id);
        Task<TEntity?> CreateAsync(TEntity item);
        Task<TEntity?> UpdateAsync(TEntity item);
        Task<TEntity?> DeleteAsync(int? id);
        Task<int> DeleteSeveralAsync(int[]? ids);
        IEnumerable<TEntity?> GetListFor(int fkId);
    }
}

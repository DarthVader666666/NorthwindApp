namespace Northwind.Bll.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity?> GetList();
        Task<TEntity?> Get(int? id);
        Task<TEntity?> Create(TEntity item);
        Task<TEntity?> Update(TEntity item);
        Task<TEntity?> Delete(int? id);
        Task<int> DeleteSeveral(int[]? ids);
    }
}

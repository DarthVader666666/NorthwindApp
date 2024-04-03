using Northwind.Data;

namespace Northwind.Bll.Interfaces
{
    public interface IGuestRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        NorthwindDbContext DbContext { get; }
    }
}

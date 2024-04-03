using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;
using Northwind.Data;

namespace Northwind.Bll.Services
{
    public class GuestCategoryRepository : RepositoryBase<Category, NorthwindInMemoryDbContext>
    {
        public GuestCategoryRepository(NorthwindInMemoryDbContext dbContext) : base(dbContext)
        {
        }
    }
}

using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public class CategoryRepository : RepositoryBase<Category, NorthwindDbContext>
    {
        public CategoryRepository(NorthwindDbContext context) : base(context)
        {
        }
    }
}

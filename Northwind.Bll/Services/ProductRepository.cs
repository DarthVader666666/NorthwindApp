using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public class ProductRepository : RepositoryBase<Product, NorthwindDbContext>
    {
        public ProductRepository(NorthwindDbContext dbContext) : base(dbContext)
        {
        }

        public override IEnumerable<Product> GetListFor(int categoryId)
        {
            return GetList().Where(x => x.CategoryId == categoryId);
        }
    }
}

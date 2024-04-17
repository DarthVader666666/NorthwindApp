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
            var products = GetList().Where(x => x.CategoryId == categoryId);
            return products;
        }

        public override async Task<int> DeleteSeveralAsync(int[]? ids)
        {
            Product product = null;

            foreach (var id in ids!)
            {
                product = await DbContext.FindAsync<Product>(id);

                if (product != null)
                {
                    DbContext.Remove(product);
                }
            }

            await DbContext.SaveChangesAsync();

            return product == null ? 0 : product.CategoryId ?? 0;
        }
    }
}

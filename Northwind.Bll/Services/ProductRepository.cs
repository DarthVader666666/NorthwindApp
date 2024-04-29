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

        public override Task<IEnumerable<Product?>> GetListForAsync(int fkId)
        {
            return Task.Run(() => DbContext.Products.Where(x => fkId == 0 || x.CategoryId == fkId).AsEnumerable());
        }

        public override Task<IEnumerable<Product?>> GetRangeAsync(params object[] ids)
        {
            return Task.Run(async () => 
            {
                var products = await GetListAsync();
                return products.Where(x => ids.Any(id => (int?)id == x.ProductId));
            });
        }

        public override async Task<int> DeleteSeveralAsync(object[] ids)
        {
            var products = await GetRangeAsync(ids);
            DbContext.RemoveRange(products);

            return await DbContext.SaveChangesAsync();
        }
    }
}

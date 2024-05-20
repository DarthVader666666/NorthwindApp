using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public override async Task<Product?> GetAsync(object? id)
        {
            return await DbContext.Products.AsNoTracking().Include(x => x.Category).Include(x => x.Supplier).FirstOrDefaultAsync(x => x.ProductId == (int?)id);
        }

        public override Task<IEnumerable<Product?>> GetListForAsync(object? foreignKeys)
        {
            var ids = (Tuple<int?, int?>)(foreignKeys ?? (0, 0));

            var products = ids switch
            {
                (> 0, null) => DbContext.Products.Where(x => ids.Item1 == null || x.CategoryId == ids.Item1),
                (null, > 0) => DbContext.Products.Where(x => ids.Item2 == null || x.SupplierId == ids.Item2),
                _ => DbContext.Products
            };

            return Task.FromResult(products?.AsEnumerable() ?? Enumerable.Empty<Product?>());
        }

        public override Task<IEnumerable<Product?>> GetRangeAsync(int[] ids)
        {
            return Task.Run(async () => 
            {
                var products = await GetListAsync();
                return products.Where(x => ids.Any(id => id == x.ProductId));
            });
        }

        public override async Task<int> DeleteSeveralAsync(int[] ids)
        {
            var products = await GetRangeAsync(ids);
            DbContext.RemoveRange(products);

            return await DbContext.SaveChangesAsync();
        }
    }
}

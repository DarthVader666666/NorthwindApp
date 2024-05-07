using Microsoft.EntityFrameworkCore;
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

        public override Task<IEnumerable<Category?>> GetRangeAsync(int[] ids)
        {
            return Task.Run(async () =>
            {
                var categories = await GetListAsync();
                return categories.Where(x => ids.Any(id => (int?)id == x.CategoryId));
            });
        }

        public override async Task<int> DeleteSeveralAsync(int[] ids)
        {
            int count = 0;

            foreach (var id in ids!)
            {
                var category = await DbContext.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.CategoryId == id);

                if (category != null)
                {
                    foreach (var product in category.Products)
                    {
                        product.CategoryId = null;
                    }

                    DbContext.Products.UpdateRange(category.Products);
                    await DbContext.SaveChangesAsync();

                    await DeleteAsync(id);
                    count++;
                }
            }

            return count;
        }
    }
}

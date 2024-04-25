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

        public override Task<IEnumerable<Category?>> GetRangeAsync(params int?[] ids)
        {
            return Task.Run(async () =>
            {
                var categories = await GetListAsync();
                return categories.Where(x => ids.Any(id => id == x.CategoryId));
            });
        }
    }
}

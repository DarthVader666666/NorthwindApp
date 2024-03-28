using Northwind.Bll.Enums;
using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public class CategoryRepository : RepositoryBase<Category>
    {
        public CategoryRepository(NorthwindDbContext context) : base(context)
        {
        }

        public override IEnumerable<Category> GetList()
        {
            foreach (Category category in base.GetList())
            {
                category.Picture = ImageConverter.ConvertNorthwindPhoto(category.Picture!, ImageHeaders.Category);
                yield return category;
            }
        }

        public override async Task<Category?> Get(int? id)
        {
            var category = await base.Get(id);
            category!.Picture = ImageConverter.ConvertNorthwindPhoto(category.Picture!, ImageHeaders.Category);

            return category;
        }
    }
}

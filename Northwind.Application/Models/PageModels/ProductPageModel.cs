namespace Northwind.Application.Models.PageModels
{
    public class ProductPageModel: PageModelBase
    {
        public int? CategoryId { get; }
        
        public ProductPageModel(int count, int pageNumber, int pageSize, int categoryId) : base(count, pageNumber, pageSize)
        {
            CategoryId = categoryId;
        }
    }
}

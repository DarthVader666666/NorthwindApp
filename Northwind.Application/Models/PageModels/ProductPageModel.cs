namespace Northwind.Application.Models.PageModels
{
    public class ProductPageModel: PageModelBase
    {
        public int? CategoryId { get; }
        public int? SupplierId { get; }
        
        public ProductPageModel(int count, int pageNumber, int pageSize, int? categoryId, int? supplierId) : base(count, pageNumber, pageSize)
        {
            CategoryId = categoryId;
            SupplierId = supplierId;
        }
    }
}

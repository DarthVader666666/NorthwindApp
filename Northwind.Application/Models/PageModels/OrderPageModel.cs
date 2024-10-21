namespace Northwind.Application.Models.PageModels
{
    public class OrderPageModel : PageModelBase
    {
        public string? CustomerId { get; }
        public int? SellerId { get; }

        public OrderPageModel(int count, int pageNumber, int pageSize, string? customerId, int? sellerId) : base(count, pageNumber, pageSize)
        {
            CustomerId = customerId;
            SellerId = sellerId;
        }
    }
}

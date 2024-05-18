namespace Northwind.Application.Models.PageModels
{
    public class OrderDetailsPageModel : PageModelBase
    {
        public int? OrderId { get; }
        public int? ProductId { get; }

        public OrderDetailsPageModel(int count, int pageNumber, int pageSize, int? orderId, int? productId) : base(count, pageNumber, pageSize)
        {
            OrderId = orderId;
            ProductId = productId;
        }
    }
}

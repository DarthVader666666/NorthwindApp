namespace Northwind.Application.Models.PageModels
{
    public class OrderPageModel : PageModelBase
    {
        public string CustomerId { get; }

        public OrderPageModel(int count, int pageNumber, int pageSize, string customerId) : base(count, pageNumber, pageSize)
        {
            CustomerId = customerId;
        }
    }
}

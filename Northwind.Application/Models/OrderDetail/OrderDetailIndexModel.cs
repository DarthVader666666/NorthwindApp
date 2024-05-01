using Northwind.Application.Models.Product;

namespace Northwind.Application.Models.OrderDetail
{
    public class OrderDetailIndexModel
    {
        public IEnumerable<OrderDetailIndexDataModel> OrderDetails { get; }
        public PageViewModel PageViewModel { get; }

        public OrderDetailIndexModel(IEnumerable<OrderDetailIndexDataModel> orders, PageViewModel viewModel)
        {
            OrderDetails = orders;
            PageViewModel = viewModel;
        }
    }
}

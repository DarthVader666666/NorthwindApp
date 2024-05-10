using Northwind.Application.Models.PageModels;
using Northwind.Application.Models.Product;

namespace Northwind.Application.Models.OrderDetail
{
    public class OrderDetailIndexModel
    {
        public IEnumerable<OrderDetailIndexDataModel> OrderDetails { get; }
        public PageModelBase PageViewModel { get; }

        public OrderDetailIndexModel(IEnumerable<OrderDetailIndexDataModel> orders, PageModelBase viewModel)
        {
            OrderDetails = orders;
            PageViewModel = viewModel;
        }
    }
}

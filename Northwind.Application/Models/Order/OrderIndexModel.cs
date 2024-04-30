using Northwind.Application.Models.Product;

namespace Northwind.Application.Models.Order
{
    public class OrderIndexModel
    {
        public IEnumerable<OrderIndexDataModel> Orders { get; }
        public PageViewModel PageViewModel { get; }

        public OrderIndexModel(IEnumerable<OrderIndexDataModel> orders, PageViewModel viewModel)
        {
            Orders = orders;
            PageViewModel = viewModel;
        }
    }
}

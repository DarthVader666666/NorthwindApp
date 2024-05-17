using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Application.Models.PageModels;
using Northwind.Application.Models.Product;

namespace Northwind.Application.Models.Order
{
    public class OrderIndexModel
    {
        public IEnumerable<OrderIndexDataModel> Orders { get; }
        public PageModelBase PageViewModel { get; }
        public SelectList? CustomerList { get; set;  }

        public OrderIndexModel(IEnumerable<OrderIndexDataModel> orders, PageModelBase viewModel)
        {
            Orders = orders;
            PageViewModel = viewModel;
        }
    }
}

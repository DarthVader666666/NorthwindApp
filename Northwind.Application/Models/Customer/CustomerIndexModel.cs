using Northwind.Application.Models.PageModels;
using Northwind.Application.Models.Product;
using Northwind.Data.Entities;

namespace Northwind.Application.Models.Customer
{
    public class CustomerIndexModel
    {
        public IEnumerable<CustomerIndexDataModel> Customers { get; }
        public PageModelBase PageViewModel { get; }

        public CustomerIndexModel(IEnumerable<CustomerIndexDataModel> customers, PageModelBase viewModel)
        {
            Customers = customers;
            PageViewModel = viewModel;
        }
    }
}

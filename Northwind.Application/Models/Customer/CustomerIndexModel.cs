using Northwind.Application.Models.Product;
using Northwind.Data.Entities;

namespace Northwind.Application.Models.Customer
{
    public class CustomerIndexModel
    {
        public IEnumerable<CustomerIndexDataModel> Customers { get; }
        public PageViewModel PageViewModel { get; }

        public CustomerIndexModel(IEnumerable<CustomerIndexDataModel> customers, PageViewModel viewModel)
        {
            Customers = customers;
            PageViewModel = viewModel;
        }
    }
}

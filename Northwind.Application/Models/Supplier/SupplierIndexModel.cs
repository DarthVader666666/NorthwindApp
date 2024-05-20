using Northwind.Application.Models.Order;
using Northwind.Application.Models.PageModels;

namespace Northwind.Application.Models.Supplier
{
    public class SupplierIndexModel
    {
        public IEnumerable<SupplierIndexDataModel> Suppliers { get; }
        public PageModelBase PageViewModel { get; }

        public SupplierIndexModel(IEnumerable<SupplierIndexDataModel> suppliers, PageModelBase viewModel)
        {
            Suppliers = suppliers;
            PageViewModel = viewModel;
        }
    }
}

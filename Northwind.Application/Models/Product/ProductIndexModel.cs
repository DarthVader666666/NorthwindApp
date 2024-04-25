namespace Northwind.Application.Models.Product
{
    public partial class ProductIndexModel
    {
        public IEnumerable<ProductIndexDataModel> Products { get; }
        public PageViewModel PageViewModel { get; }

        public ProductIndexModel(IEnumerable<ProductIndexDataModel> products, PageViewModel viewModel)
        {
            Products = products;
            PageViewModel = viewModel;
        }
    }
}

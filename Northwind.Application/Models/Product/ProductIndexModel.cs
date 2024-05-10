﻿using Northwind.Application.Models.PageModels;

namespace Northwind.Application.Models.Product
{
    public partial class ProductIndexModel
    {
        public IEnumerable<ProductIndexDataModel> Products { get; }
        public PageModelBase PageViewModel { get; }

        public ProductIndexModel(IEnumerable<ProductIndexDataModel> products, PageModelBase viewModel)
        {
            Products = products;
            PageViewModel = viewModel;
        }
    }
}

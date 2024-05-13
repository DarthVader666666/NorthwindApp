﻿namespace Northwind.Application.Models.Product
{
    public class ProductIndexDataModel
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? QuantityPerUnit { get; set; }

        public short? UnitsInStock { get; set; }

        public decimal? UnitPrice { get; set; }
    }
}

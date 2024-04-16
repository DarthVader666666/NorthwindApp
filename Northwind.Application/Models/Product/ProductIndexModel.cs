﻿namespace Northwind.Application.Models.Product
{
    public partial class ProductIndexModel
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? QuantityPerUnit { get; set; }

        public decimal? UnitPrice { get; set; }

        public short? UnitsInStock { get; set; }
    }
}

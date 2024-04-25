using System.Collections;

namespace Northwind.Application.Models.Product
{
    public class ProductDeleteModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? QuantityPerUnit { get; set; }

        public string? UnitsInStock { get; set; }
    }
}

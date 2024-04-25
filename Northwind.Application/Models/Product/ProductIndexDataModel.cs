namespace Northwind.Application.Models.Product
{
    public class ProductIndexDataModel
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? QuantityPerUnit { get; set; }

        public string? UnitPrice { get; set; }

        public string? UnitsInStock { get; set; }
    }
}

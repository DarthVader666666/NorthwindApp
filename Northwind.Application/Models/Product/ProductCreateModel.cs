namespace Northwind.Application.Models.Product
{
    public class ProductCreateModel
    {
        public string ProductName { get; set; } = null!;

        public int? CategoryId { get; set; }

        public int? SupplierId { get; set; }

        public string? QuantityPerUnit { get; set; }

        public decimal? UnitPrice { get; set; }

        public short? UnitsInStock { get; set; }
    }
}

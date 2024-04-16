namespace Northwind.Application.Models.Product
{
    public partial class ProductIndexModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? QuantityPerUnit { get; set; }

        public decimal? UnitPrice { get; set; }

        public short? UnitsInStock { get; set; }

        public string? CategoryName { get; set; }
    }
}

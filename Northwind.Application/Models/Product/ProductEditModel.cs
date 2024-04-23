namespace Northwind.Application.Models.Product
{
    public class ProductEditModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int SupplierId { get; set; }

        public int CategoryId { get; set; }

        public ushort? UnitsOnOrder { get; set; }

        public ushort? ReorderLevel { get; set; }

        public string? QuantityPerUnit { get; set; }

        public decimal? UnitPrice { get; set; }

        public ushort? UnitsInStock { get; set; }

        public bool Discontinued { get; set; }
    }
}

using Northwind.Data.Entities;

namespace Northwind.Application.Models.Product
{
    public class ProductDetailsModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int? SupplierId { get; set; }

        public int? CategoryId { get; set; }

        public string? QuantityPerUnit { get; set; }

        public string? UnitPrice { get; set; }

        public string? UnitsInStock { get; set; }

        public string? UnitsOnOrder { get; set; }

        public string? ReorderLevel { get; set; }

        public string Discontinued { get; set; }

        public Northwind.Data.Entities.Category? Category { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public Supplier? Supplier { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Northwind.Application.Models.Product
{
    public class ProductCreateModel
    {
        public string ProductName { get; set; } = null!;

        public int? CategoryId { get; set; }

        public int? SupplierId { get; set; }

        public ushort? QuantityPerUnit { get; set; }

        public decimal? UnitPrice { get; set; }

        public ushort? UnitsInStock { get; set; }

        public SelectList? CategoryIdList { get; set; }

        public SelectList? SupplierIdList { get; set; }
    }
}

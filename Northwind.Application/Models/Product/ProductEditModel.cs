using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models.Product
{
    public class ProductEditModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        public string ProductName { get; set; } = null!;

        public int? Supplier { get; set; }

        public int? Category { get; set; }

        public string? QuantityPerUnit { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Value out of range")]
        public short? UnitsOnOrder { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Value out of range")]
        public short? ReorderLevel { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Value out of range")]
        public decimal? UnitPrice { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Value out of range")]
        public short? UnitsInStock { get; set; }

        public bool Discontinued { get; set; }

        public SelectList? CategoryList { get; set; }

        public SelectList? SupplierList { get; set; }
    }
}

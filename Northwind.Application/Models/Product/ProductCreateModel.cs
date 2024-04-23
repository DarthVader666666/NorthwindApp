using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models.Product
{
    public class ProductCreateModel
    {
        [Required(ErrorMessage = "Product Name is required")]
        public string ProductName { get; set; } = null!;

        public int? CategoryId { get; set; }

        public int? SupplierId { get; set; }

        public string? QuantityPerUnit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Value out of range")]
        public decimal? UnitPrice { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Value out of range")]
        public short? UnitsInStock { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Value out of range")]
        public short? UnitsOnOrder { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Value out of range")]
        public short? ReorderLevel { get; set; }

        public SelectList? CategoryIdList { get; set; }

        public SelectList? SupplierIdList { get; set; }
    }
}

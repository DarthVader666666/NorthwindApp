using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models.Supplier
{
    public class SupplierCreateModel
    {
        public int SupplierId { get; set; }
        [Required]
        public string CompanyName { get; set; } = null!;

        public string? ContactName { get; set; }

        public string? ContactTitle { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public string? City { get; set; }

        public string? Region { get; set; }

        public string? PostalCode { get; set; }

        public string? Country { get; set; }

        public string? Phone { get; set; }

        public string? Fax { get; set; }

        public string? HomePage { get; set; }

    }
}

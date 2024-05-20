namespace Northwind.Application.Models.Supplier
{
    public class SupplierIndexDataModel
    {
        public int SupplierId { get; set; }

        public string CompanyName { get; set; } = null!;

        public string? City { get; set; }

        public string? Country { get; set; }
    }
}

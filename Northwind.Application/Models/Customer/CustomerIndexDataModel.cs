namespace Northwind.Application.Models.Customer
{
    public class CustomerIndexDataModel
    {
        public string CustomerId { get; set; } = null!;

        public string CompanyName { get; set; } = null!;

        public string? ContactName { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }
    }
}

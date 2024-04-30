using Northwind.Data.Entities;

namespace Northwind.Application.Models.Order
{
    public class OrderIndexDataModel
    {
        public int OrderId { get; set; }

        public string? CustomerId { get; set; }

        public int? EmployeeId { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public string? ShipName { get; set; }

        public string? ShipCity { get; set; }

        public string? ShipCountry { get; set; }
    }
}

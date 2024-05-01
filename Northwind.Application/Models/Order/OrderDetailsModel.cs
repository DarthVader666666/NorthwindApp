using Northwind.Data.Entities;

namespace Northwind.Application.Models.Order
{
    public class OrderDetailsModel
    {
        public int OrderId { get; set; }

        public string? CustomerId { get; set; }

        public int? EmployeeId { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public int? ShipperId { get; set; }

        public decimal? Freight { get; set; }

        public string? ShipName { get; set; }

        public string? ShipAddress { get; set; }

        public string? ShipCity { get; set; }

        public string? ShipRegion { get; set; }

        public string? ShipPostalCode { get; set; }

        public string? ShipCountry { get; set; }

        public virtual ICollection<Northwind.Data.Entities.OrderDetail> OrderDetails { get; set; } = new List<Northwind.Data.Entities.OrderDetail>();
    }
}

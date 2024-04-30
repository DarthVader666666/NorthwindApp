using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models.Order
{
    public class OrderCreateModel
    {
        [Required(ErrorMessage = "Please, choose Customer")]
        public string? CustomerId { get; set; }

        public int? EmployeeId { get; set; }

        public DateTime? OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public int? ShipperId { get; set; }

        [Range(0, 9999999999.99, ErrorMessage = "Only value 0...9999999999.99 allowed")]
        public decimal? Freight { get; set; }

        public string? ShipName { get; set; }

        public string? ShipAddress { get; set; }

        public string? ShipCity { get; set; }

        public string? ShipRegion { get; set; }

        public string? ShipPostalCode { get; set; }

        public string? ShipCountry { get; set; }

        public SelectList? EmployeeIdList { get; set; }

        public SelectList? CustomerIdList { get; set; }

        public SelectList? ShipperIdList { get; set; }
    }
}

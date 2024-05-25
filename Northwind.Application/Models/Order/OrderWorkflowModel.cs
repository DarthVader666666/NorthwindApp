using Microsoft.AspNetCore.Mvc.Rendering;

namespace Northwind.Application.Models.Order
{
    public class OrderWorkflowModel
    {
        public int OrderId { get; set; }

        public int? EmployeeId { get; set; }

        public string? OrderStatus { get; set; }

        public decimal? TotalCost { get; set; }

        public SelectList? EmployeeList { get; set; }
    }
}

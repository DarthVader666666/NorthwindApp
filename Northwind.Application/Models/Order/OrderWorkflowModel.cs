using Microsoft.AspNetCore.Mvc.Rendering;

namespace Northwind.Application.Models.Order
{
    public class OrderWorkflowModel
    {
        public int? OrderNumber { get; set; }

        public int? EmployeeId { get; set; }

        public string? OrderStatus { get; set; }

        public SelectList? EmployeeList { get; set; }
    }
}

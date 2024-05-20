namespace Northwind.Application.Models.Order
{
    public class OrderIndexDataModel
    {
        public int OrderId { get; set; }

        public string? CustomerId { get; set; }

        public int? EmployeeId { get; set; }

        public DateTime? OrderDate { get; set; }

        public string? OrderStatus { get; set; }

        public decimal? TotalCost { get; set; }
    }
}

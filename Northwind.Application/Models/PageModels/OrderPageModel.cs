namespace Northwind.Application.Models.PageModels
{
    public class OrderPageModel : PageModelBase
    {
        public string? CustomerId { get; }
        public int? EmployeeId { get; }

        public OrderPageModel(int count, int pageNumber, int pageSize, string? customerId, int? employeeId) : base(count, pageNumber, pageSize)
        {
            CustomerId = customerId;
            EmployeeId = employeeId;
        }
    }
}

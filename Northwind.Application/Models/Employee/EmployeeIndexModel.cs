namespace Northwind.Application.Models.Employee
{
    public class EmployeeIndexModel
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; } = null!;

        public string? Title { get; set; }

        public byte[]? Photo { get; set; }
    }
}

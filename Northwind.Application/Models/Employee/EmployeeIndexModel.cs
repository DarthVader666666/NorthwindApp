namespace Northwind.Application.Models.Employee
{
    public class EmployeeIndexModel
    {
        public int EmployeeId { get; set; }

        public string LastName { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string? Title { get; set; }

        public string? HireDate { get; set; }

        public byte[]? Photo { get; set; }
    }
}

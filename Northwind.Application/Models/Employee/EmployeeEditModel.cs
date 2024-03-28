using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models.Employee
{
    public class EmployeeEditModel
    {
        public int EmployeeId { get; set; }
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string? Title { get; set; }

        public string? TitleOfCourtesy { get; set; }
        [Required]
        public DateTime? BirthDate { get; set; }
        [Required]
        public DateTime? HireDate { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Region { get; set; }

        public string? PostalCode { get; set; }

        public string? Country { get; set; }

        public string? HomePhone { get; set; }

        public byte[]? Photo { get; set; }

        public IFormFile? FormFile { get; set; }

        public string? Notes { get; set; }

        public int? ReportsTo { get; set; }
    }
}

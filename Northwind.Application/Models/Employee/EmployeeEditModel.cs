using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Bll.Validation;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models.Employee
{
    public class EmployeeEditModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = null!;

        public string? Title { get; set; }

        public string? TitleOfCourtesy { get; set; }

        [DateValidation(MinYears = 18, MaxYears = 90)]
        public DateTime? BirthDate { get; set; }

        [DateValidation(MinYears = 0, MaxYears = 60)]
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

        public SelectList? ReportsToList { get; set; }
    }
}

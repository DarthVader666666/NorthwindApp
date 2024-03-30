﻿using Northwind.Bll.Validation;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models.Employee
{
    public class EmployeeCreateModel
    {
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string? Title { get; set; }

        public string? TitleOfCourtesy { get; set; }
        [Required]
        [DateValidation(MinYears = 18, MaxYears = 60)]
        public DateTime? BirthDate { get; set; }
        [Required]
        [DateValidation(MinYears = 0, MaxYears = 60)]
        public DateTime? HireDate { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Region { get; set; }

        public string? PostalCode { get; set; }

        public string? Country { get; set; }

        public string? HomePhone { get; set; }

        public IFormFile? FormFile { get; set; }

        public string? Notes { get; set; }

        public int? ReportsTo { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Northwind.Application.Models.Roles
{
    public class RoleChangeModel
    {
        public string? UserId { get; set; }
        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public string? UserEmail { get; set; }
        public List<IdentityRole> AllRoles { get; set; } = new List<IdentityRole>();
        public IList<string> UserRoles { get; set; } = new List<string>();
        public SelectList? CustomerList { get; set; }
        public SelectList? EmployeeList { get; set; }
    }
}

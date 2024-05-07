using Microsoft.AspNetCore.Identity;

namespace Northwind.Application.Models.Roles
{
    public class RoleChangeModel
    {
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public List<IdentityRole> AllRoles { get; set; } = new List<IdentityRole>();
        public IList<string> UserRoles { get; set; } = new List<string>();
    }
}

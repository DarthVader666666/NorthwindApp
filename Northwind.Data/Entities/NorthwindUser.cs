using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Data.Entities
{
    public class NorthwindUser : IdentityUser
    {
        [StringLength(5, MinimumLength = 5)]
        public string? CustomerId { get; set; }
    }
}

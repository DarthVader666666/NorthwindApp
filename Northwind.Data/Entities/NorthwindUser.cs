using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Data.Entities
{
    public class NorthwindUser: IdentityUser
    {
        [ForeignKey(nameof(CustomerId))]
        public string? CustomerId { get; set; } = null;

        public Customer? Customer { get; set; }
    }
}

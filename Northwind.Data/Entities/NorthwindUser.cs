using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Data.Entities
{
    public class NorthwindUser: IdentityUser
    {
        [ForeignKey(nameof(CustomerId))]
        public string? CustomerId { get; set; } = null;

        [ForeignKey(nameof(EmployeeId))]
        public int? EmployeeId { get; set; } = null;

        public virtual Customer? Customer { get; set; }

        public virtual Seller? Employee { get; set; }
    }
}

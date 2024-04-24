using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Northwind.Data.Entities;

namespace Northwind.Data;

public class NorthwindIdentityDbContext : IdentityDbContext<NorthwindUser>
{
    public NorthwindIdentityDbContext(DbContextOptions<NorthwindIdentityDbContext> options)
        : base(options)
    {
    }
}

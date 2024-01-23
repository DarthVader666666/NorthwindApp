using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Northwind.Data
{
    public class NorthwindDbContext : DbContext
    {
        public NorthwindDbContext(DbContextOptions<NorthwindDbContext> dbContextOptions) : base(dbContextOptions)
        {
            this.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
        }
    }
}

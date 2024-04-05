using Microsoft.EntityFrameworkCore;

namespace Northwind.Data
{
    public class NorthwindInMemoryDbContext : NorthwindDbContext
    {
        public NorthwindInMemoryDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("NorthwindInMemoryDb");
        }
    }
}

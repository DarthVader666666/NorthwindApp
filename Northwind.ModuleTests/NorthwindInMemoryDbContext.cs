using Microsoft.EntityFrameworkCore;

namespace Northwind.Data
{
    public class NorthwindInMemoryDbContext : NorthwindDbContext
    {
        public NorthwindInMemoryDbContext(DbContextOptions<NorthwindDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("NorthwindInMemoryDb");
        }
    }
}

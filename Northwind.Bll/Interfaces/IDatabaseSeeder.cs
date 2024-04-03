using Northwind.Data;

namespace Northwind.Bll.Interfaces
{
    public interface IDatabaseSeeder
    {
        Task SeedDatabase<TDbContext>() where TDbContext : NorthwindDbContext;
    }
}
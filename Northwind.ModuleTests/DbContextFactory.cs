using Northwind.Data;

namespace Northwind.ModuleTests
{
    public static class DbContextFactory
    {
        public static NorthwindInMemoryDbContext GetInMemoryDbContext()
        {
            return new NorthwindInMemoryDbContext();
        }
    }
}

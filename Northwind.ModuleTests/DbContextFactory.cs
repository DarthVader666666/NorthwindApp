using Northwind.Data;

namespace Northwind.ModuleTests
{
    public class DbContextFactory
    {
        private readonly NorthwindInMemoryDbContext _northwindInMemoryDbContext;

        public DbContextFactory()
        {
            _northwindInMemoryDbContext = new NorthwindInMemoryDbContext();
        }

        public NorthwindInMemoryDbContext GetInMemoryDbContext()
        {
            return _northwindInMemoryDbContext;
        }
    }
}

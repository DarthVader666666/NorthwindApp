using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public class GuestEmployeeRepository : RepositoryBase<Employee, NorthwindInMemoryDbContext>
    {
        public GuestEmployeeRepository(NorthwindInMemoryDbContext dbContext) : base (dbContext)
        {
        }
    }
}

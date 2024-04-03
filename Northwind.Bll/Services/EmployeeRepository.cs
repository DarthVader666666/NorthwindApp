using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public class EmployeeRepository : RepositoryBase<Employee, NorthwindDbContext>
    {
        public EmployeeRepository(NorthwindDbContext context) : base (context)
        {
        }
    }
}

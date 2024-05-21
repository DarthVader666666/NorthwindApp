using Microsoft.EntityFrameworkCore;
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

        public async override Task<Employee?> GetAsync(object id)
        {
            return await DbContext.Employees.Include(e => e.ReportsToNavigation).FirstOrDefaultAsync(e => e.EmployeeId == (int?)id);
        }
    }
}

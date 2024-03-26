using Northwind.Bll.Enums;
using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;

namespace Northwind.Bll.Services
{
    public class EmployeeRepository : RepositoryBase<Employee>
    {
        public EmployeeRepository(NorthwindDbContext context) : base (context)
        {
        }

        public override IEnumerable<Employee> GetList()
        {
            foreach (Employee employee in base.GetList()) 
            {
                employee.Photo = ImageConverter.ConvertNorthwindPhoto(employee.Photo!, ImageHeaders.Employee);
                yield return employee;
            }
        }

        public override async Task<Employee?> Get(int? id)
        { 
            var employee = await base.Get(id);
            employee!.Photo = ImageConverter.ConvertNorthwindPhoto(employee.Photo!, ImageHeaders.Employee);

            return employee;
        }
    }
}

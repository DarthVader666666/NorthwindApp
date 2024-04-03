using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles = "admin,user")]
    public class EmployeesController : EmployeesControllerBase
    {
        public EmployeesController(IRepository<Employee> employeeRepository, IMapper mapper) : base (employeeRepository, mapper)
        {
        }
    }
}

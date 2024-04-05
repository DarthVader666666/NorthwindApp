using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Northwind.Application.Controllers;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data.Entities;

namespace Northwind.Application.GuestControllers
{
    [Authorize(Roles = "guest")]
    public class GuestEmployeesController : EmployeesControllerBase
    {
        public GuestEmployeesController(IGuestRepository<Employee> employeeRepository, IMapper mapper) : base(employeeRepository, mapper)
        {
        }
    }
}

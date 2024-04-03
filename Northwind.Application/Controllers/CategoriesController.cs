using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles = "admin,user")]
    public class CategoriesController : CategoriesControllerBase
    {
        public CategoriesController(IRepository<Category> categoryRepository, IMapper mapper) : base (categoryRepository, mapper)
        {
        }
    }
}

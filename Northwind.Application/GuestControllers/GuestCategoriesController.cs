using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Northwind.Application.Controllers;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data.Entities;

namespace Northwind.Application.GuestControllers
{
    [Authorize(Roles = "guest")]
    public class GuestCategoriesController : CategoriesControllerBase
    {
        public GuestCategoriesController(IGuestRepository<Category> categoryRepository, IMapper mapper) : base(categoryRepository, mapper)
        {
        }
    }
}

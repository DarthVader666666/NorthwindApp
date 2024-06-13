using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Constants;
using Northwind.Application.Services;
using Northwind.Data.Entities;
using NorthwindApp.Models;
using System.Diagnostics;

namespace NorthwindApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly RolesConfigurator _roleConfigurator;

        public HomeController(RolesConfigurator roleConfigurator)
        {
            _roleConfigurator = roleConfigurator;
        }

        public async Task<IActionResult> Index()
        {
            await _roleConfigurator.ConfigureRolesAsync();

            return RedirectToAction("Index", "Categories");
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Northwind.Data.Entities;
using NorthwindApp.Models;
using System.Diagnostics;

namespace NorthwindApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly UserManager<NorthwindUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(IConfiguration config, UserManager<NorthwindUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            if (await IsProduction())
            {
                var user = new NorthwindUser
                {
                    UserName = _config["OwnerEmail"],
                    Email = _config["OwnerEmail"],
                    PasswordHash = _config["OwnerPasswordHash"],
                    SecurityStamp = _config["OwnerSecurityStamp"],
                    ConcurrencyStamp = _config["OwnerConcurrencyStamp"],
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(user);
                await _roleManager.CreateAsync(new IdentityRole { Name = "owner", NormalizedName = "OWNER", ConcurrencyStamp = null });
                await _userManager.AddToRoleAsync(user, "owner");
            }

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

        private async Task<bool> IsProduction()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" 
                && _config["OwnerEmail"] != null 
                && await _userManager.FindByNameAsync(_config["OwnerEmail"]!) == null;
        }
    }
}

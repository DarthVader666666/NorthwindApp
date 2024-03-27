using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Data;
using NorthwindApp.Models;
using System.Diagnostics;

namespace NorthwindApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindDbContext _db;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, NorthwindDbContext db, IConfiguration config)
        {
            _logger = logger;
            _db = db;
            _config = config;
        }

        public IActionResult Index()
        {
            ViewData["Email"] = _config["Email"];
            return View();
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

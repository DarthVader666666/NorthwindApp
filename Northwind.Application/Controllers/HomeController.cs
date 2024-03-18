using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Data;
using NorthwindApp.Identity;
using NorthwindApp.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NorthwindApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindDbContext _db;
        private readonly IConfiguration _config;
        private static byte[] OleHeader = new byte[] { 21, 28, 47, 0, 2, 0, 0, 0, 13, 0, 14, 0, 20,
            0, 33, 0, 255, 255, 255, 255, 66, 105, 116, 109, 97, 112, 32, 73, 109, 97, 103, 101, 0, 80, 97,
            105, 110, 116, 46, 80, 105, 99, 116, 117, 114, 101, 0, 1, 5, 0, 0, 2, 0, 0, 0, 7, 0, 0, 0, 80,
            66, 114, 117, 115, 104, 0, 0, 0, 0, 0, 0, 0, 0, 0, 32, 84, 0, 0 };

        public HomeController(ILogger<HomeController> logger, NorthwindDbContext db, IConfiguration config)
        {
            _logger = logger;
            _db = db;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [RequiresClaim("admin", "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Authorize()
        {
            return View();
        }

        public async Task<IActionResult> GetToken([FromForm] AuthorizationViewModel model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["SecretKey"]);

            var claims = new List<Claim>
            {
                new (ClaimTypes.Role, model.UserRole)
            };

            var claimsIdentity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimsPrincipal);

            return new OkObjectResult(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> GetEmployees()
        {
            var employees = _db.Employees;
            await employees.ForEachAsync(x => x.Photo = ConvertNorthwindPhoto(x.Photo!));

            return View(employees);
        }

        public static bool HasHeader(byte[] source, byte[] header)
        {
            if (source.Length < header.Length)
            {
                return false;
            }

            for (int i = 0; i < header.Length; i++)
            {
                if (source[i] != header[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static byte[] ConvertNorthwindPhoto(byte[] source) =>
            HasHeader(source, OleHeader) ? source[OleHeader.Length..] : source;
    }
}

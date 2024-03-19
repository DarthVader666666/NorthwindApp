using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}

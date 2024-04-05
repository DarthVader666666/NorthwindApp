using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Northwind.Data;

namespace Northwind.Application.Controllers
{
    public class GuestController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly NorthwindInMemoryDbContext _dbContext;

        public GuestController(NorthwindInMemoryDbContext dbContext, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Register()
        {
            var user = Activator.CreateInstance<IdentityUser>();

            user.UserName = "Guest" + Guid.NewGuid().ToString();
            user.Email = Guid.NewGuid().ToString() + "@guest.com";
            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, "Guest" + Guid.NewGuid().ToString().Trim('-'));

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "guest");
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return LocalRedirect(Url.Content("~/"));
        }
    }
}

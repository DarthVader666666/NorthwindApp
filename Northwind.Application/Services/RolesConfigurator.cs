using Microsoft.AspNetCore.Identity;
using Northwind.Application.Constants;
using Northwind.Data.Entities;

namespace Northwind.Application.Services
{
    public class RolesConfigurator
    {
        private readonly IConfiguration _config;
        private readonly UserManager<NorthwindUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesConfigurator(IConfiguration config, UserManager<NorthwindUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task ConfigureRolesAsync()
        {
            if (!await IsProduction())
            {
                return;
            }

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
            await _roleManager.CreateAsync(new IdentityRole { Name = UserRoles.Owner, NormalizedName = UserRoles.Owner.ToUpper(), ConcurrencyStamp = null });
            await _userManager.AddToRoleAsync(user, UserRoles.Owner);

            user = new NorthwindUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(user, "Admin_1");
            await _roleManager.CreateAsync(new IdentityRole { Name = UserRoles.Admin, NormalizedName = UserRoles.Admin.ToUpper(), ConcurrencyStamp = null });
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);

            user = new NorthwindUser
            {
                UserName = "customer@customer.com",
                Email = "customer@customer.com",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(user, "Customer_1");
            await _roleManager.CreateAsync(new IdentityRole { Name = UserRoles.Customer, NormalizedName = UserRoles.Customer.ToUpper(), ConcurrencyStamp = null });
            await _userManager.AddToRoleAsync(user, UserRoles.Customer);
        }

        private async Task<bool> IsProduction()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production"
                && _config["OwnerEmail"] != null
                && await _userManager.FindByNameAsync(_config["OwnerEmail"]!) == null;
        }
    }
}

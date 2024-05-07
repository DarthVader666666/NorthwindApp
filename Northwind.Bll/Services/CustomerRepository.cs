using Microsoft.AspNetCore.Identity;
using Northwind.Bll.Interfaces;
using Northwind.Data;
using Northwind.Data.Entities;
using System.Security.Claims;

namespace Northwind.Bll.Services
{
    public class CustomerRepository : RepositoryBase<Customer, NorthwindDbContext>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerRepository(NorthwindDbContext dbContext, UserManager<IdentityUser> userManager) : base(dbContext)
        {
            _userManager = userManager;
        }

        public async Task<Customer?> GetUserCustomer(ClaimsPrincipal claims)
        { 
            var user = await _userManager.GetUserAsync(claims);

            return user == null ? null : DbContext.Customers.FirstOrDefault(x => x.UserId == user.Id);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Models.Roles;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data;
using Northwind.Data.Entities;
using System.Linq;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<NorthwindUser> _userManager;
        private readonly IRepository<Customer> _customerRepository;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<NorthwindUser> userManager, IRepository<Customer> customerRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _customerRepository = customerRepository;
        }

        public IActionResult Index() => View(_roleManager.Roles.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UserList()
        {
            var users = _userManager.Users.ToList();

            IEnumerable<UserIndexModel> GetUserModels()
            {
                foreach (var user in users)
                {
                    yield return new UserIndexModel { Id = user.Id, UserName = user.UserName, RoleNames = _userManager.GetRolesAsync(user).Result };
                }
            }

            return View(GetUserModels()); 
        }

        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();

                RoleChangeModel model = new RoleChangeModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles,
                    CustomerList = await GetCustomerIdSelectList(userId)
                };

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleChangeModel? roleChangeModel)
        {
            var user = await _userManager.FindByIdAsync(roleChangeModel?.UserId);

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var addedRoles = roleChangeModel.UserRoles.Except(userRoles);
                var removedRoles = userRoles.Except(roleChangeModel.UserRoles);

                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                user.CustomerId = roleChangeModel.CustomerId;
                await _userManager.UpdateAsync(user);

                return RedirectToAction("UserList");
            }

            return NotFound();
        }

        private async Task<SelectList> GetCustomerIdSelectList(string userId)
        {
            var customers = _customerRepository.GetListAsync().Result;
            var dictionary = customers.ToDictionary(c => c.CustomerId, c => c.CompanyName);

            var selectList = new SelectList(dictionary, "Key", "Value", dictionary);

            var user = await _userManager.FindByIdAsync(userId);
            var customerId = user?.CustomerId;

            SelectListItem? selectedItem = null;

            if (customerId != null)
            {
                selectedItem = selectList.FirstOrDefault(x => x.Value == customerId.ToString());
            }

            if (selectedItem != null)
            {
                selectedItem.Selected = true;
            }

            return selectList;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Application.Constants;
using Northwind.Application.Models.Roles;
using Northwind.Application.Services;
using Northwind.Bll.Interfaces;
using Northwind.Data.Entities;

namespace Northwind.Application.Controllers
{
    [Authorize(Roles = UserRoles.Owner)]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<NorthwindUser> _userManager;
        private readonly ISelectListFiller _selectListFiller;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<NorthwindUser> userManager, ISelectListFiller selectListFiller)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _selectListFiller = selectListFiller;
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

        public IActionResult UserList()
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
                    AllRoles = allRoles
                };

                _selectListFiller.FillSelectLists(model, userId: userId);

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleChangeModel? roleChangeModel)
        {
            var user = await _userManager.FindByIdAsync(roleChangeModel?.UserId ?? "");

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var addedRoles = roleChangeModel?.UserRoles.Except(userRoles);
                var removedRoles = userRoles.Except(roleChangeModel.UserRoles);

                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                user.CustomerId = roleChangeModel.CustomerId;
                user.EmployeeId = roleChangeModel.SellerId;
                await _userManager.UpdateAsync(user);

                return RedirectToAction("UserList");
            }

            return NotFound();
        }
    }
}

using ClercSystem.Areas.Admin.Models.UserManagement;
using ClercSystem.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClercSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public UserManagementController(UserManager<ApplicationUser> _userManager, 
                                        RoleManager<IdentityRole<Guid>> _roleManager)
        {
            this.userManager = _userManager;
            this.roleManager = _roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = userManager.Users.ToList();
            List<UserViewModel> userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName, 
                    LastName = user.LastName,
                    DepartmentId = user.DepartmentId,
                    IsManager = user.IsManager,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(Guid userId, string role) // assign role to user
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null) // User not found error handling
            {
                return NotFound();
            }

            var userIsLockedOut = await userManager.IsLockedOutAsync(user);
            if (userIsLockedOut)
            {
                TempData["Message"] = "Cannot assign roles to a locked out user. Please unlock the user before assigning roles.";
                return RedirectToAction(nameof(Index));
            }

            if (!await roleManager.RoleExistsAsync(role)) // Role not found error handling
            {
                return NotFound();
            }

            var result = await userManager.AddToRoleAsync(user, role);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(Guid userId, string role) // remove role from user
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null) // User not found error handling
            {
                return NotFound();
            }

            var userIsLockedOut = await userManager.IsLockedOutAsync(user);
            if (userIsLockedOut)
            {
                TempData["Message"] = "Cannot assign roles to a locked out user. Please unlock the user before assigning roles.";
                return RedirectToAction(nameof(Index));
            }

            if (!await roleManager.RoleExistsAsync(role)) // Role not found error handling
            {
                return NotFound();
            }

            var result = await userManager.RemoveFromRoleAsync(user, role);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            return BadRequest(result.Errors);
        }

        // Implementing user deletion functionality
        public async Task<IActionResult> DeleteUser(Guid userId) // delete user
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null) // User not found error handling
            {
                return NotFound();
            }

            var userRoles = await userManager.GetRolesAsync(user);

            if (userRoles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                TempData["Message"] = "Cannot delete an admin user. Please remove the admin role before deletion.";
                return RedirectToAction(nameof(Index));
            }

            user.LockoutEnabled = true; // Lock the user account before deletion to prevent access
                                        // make sure that user does not have any documents related to prevent accidental deletion of admin accounts and data loss
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(1000); // Set lockout end date far in the future to effectively lock the account
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Message"] = "User has been locked and marked for deletion. Please review and confirm deletion if necessary.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "An error occurred while trying to delete the user. Please try again.";
            return BadRequest(result.Errors);
        }

    }
}

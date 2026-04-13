using ClercSystem.Areas.Admin.ViewModels.UserManagement;
using ClercSystem.Data.Models;
using ClercSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClercSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly IDepartmentService departmentService;

        public UserManagementController(UserManager<ApplicationUser> _userManager,
                                        RoleManager<IdentityRole<Guid>> _roleManager,
                                        IDepartmentService _departmentService)
        {
            this.userManager = _userManager;
            this.roleManager = _roleManager;
            this.departmentService = _departmentService;
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
                    DepartmentId = user.DepartmentId.ToString(),
                    Departments = await this.departmentService.GetAllDepartmentsAsync(),
                    IsManager = user.IsManager ? "true" : "false",
                    Email = user.Email,
                    LockoutEnd = user.LockoutEnd != null ? "not null" : null ,
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

            var currentUser = userManager.GetUserId(User);

            if (currentUser.ToString() == user.Id.ToString()) // prevents user from deassigning roles to himself
            {
                TempData["Message"] = "You cannot assign your own role";
                return RedirectToAction(nameof(Index));
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

            return BadRequest(); // left on purpose to show example for bad request if role assigne twice gores to bad request
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(Guid userId, string role) // remove role from user
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null) // User not found error handling
            {
                return NotFound();
            }

            var currentUser = userManager.GetUserId(User);

            if (currentUser.ToString() == user.Id.ToString()) // prevents user from deassigning roles to himself
            {
                TempData["Message"] = "You cannot de-assign your own role";
                return RedirectToAction(nameof(Index));
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

        [HttpPost]
        public async Task<IActionResult> AssignManager(string userId, string IsManager) // assign manager position to user
        {
            bool isvalidGuidUser = Guid.TryParse(userId, out Guid parsedUserId);
            if (!isvalidGuidUser) // Invalid GUID format error handling
            {
                TempData["Message"] = "Invalid user ID. Please provide valid GUID.";
                return RedirectToAction(nameof(Index));
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(IsManager)) // User not found or manager property is empty error handling
            {
                TempData["Message"] = "User not found or manager property is empty. Please try again.";
                return RedirectToAction(nameof(Index));
            }

            var userRoles = await userManager.GetRolesAsync(user);
            if (userRoles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase))) // cant change if user has admin role
            {                                                                           // admin user has full rights
                TempData["Message"] = "Cant change on admin user";
                return RedirectToAction(nameof(Index));
            }

            var userIsLockedOut = await userManager.IsLockedOutAsync(user); // cant change manager status if user is locked out
            if (userIsLockedOut)
            {
                TempData["Message"] = "Cannot assign manager position to a locked out user. Please unlock the user before assigning roles.";
                return RedirectToAction(nameof(Index));
            }

            bool currentManagerStatus = user.IsManager;

            if(currentManagerStatus.ToString().ToLower() == IsManager.ToLower()) // Check if the current manager status is the
                                                                                 // same as the new value to avoid unnecessary updates
            {
                TempData["Message"] = "The user's manager status is already set to the specified value. No changes were made.";
                return RedirectToAction(nameof(Index));
            }



            if (IsManager == "true".ToLower())
            {
                user.IsManager = true;
            } else
            {
                user.IsManager = false;
            }


            var existingClaim = (await userManager.GetClaimsAsync(user)) // updating claims 
            .FirstOrDefault(c => c.Type == "IsManager");

            if (existingClaim != null)
            {
                await userManager.RemoveClaimAsync(user, existingClaim); // remove old claim
            }

            // add updated claim
            await userManager.AddClaimAsync( // add claim
                user,
                new Claim("IsManager", IsManager)
            );

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Message"] = "User manager status has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "An error occurred while trying to update the user's manager status. Please try again.";
            return BadRequest(result.Errors);

        }

        [HttpPost]
        public async Task<IActionResult> AssignDepartment(string userId, string departmentId) // assign department to user
        {
            bool isvalidGuidUser = Guid.TryParse(userId, out Guid parsedUserId);
            bool isValidGuidDepartment = Guid.TryParse(departmentId, out Guid parsedDepartmentId);

            if(!isvalidGuidUser || !isValidGuidDepartment) // Invalid GUID format error handling
            {
                TempData["Message"] = "Invalid user ID or department ID format. Please provide valid GUIDs.";
                return RedirectToAction(nameof(Index));
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrWhiteSpace(departmentId)) // User not found error handling
            {
                TempData["Message"] = "User not found. Please try again.";
                return RedirectToAction(nameof(Index));
            }

            bool doesDepartmentExist = await departmentService.DepartmentExistsByIdAsync(Guid.Parse(departmentId));
            if(!doesDepartmentExist) // Department not found error handling
            {
                TempData["Message"] = "Department not found. Please try again.";
                return RedirectToAction(nameof(Index));
            }

            var userRoles = await userManager.GetRolesAsync(user);
            if (userRoles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase))) // cant change if user has admin role
            {                                                                           // admin user has full rights
                TempData["Message"] = "Cant change on admin user";
                return RedirectToAction(nameof(Index));
            }
            var userIsLockedOut = await userManager.IsLockedOutAsync(user); // cant change department if user is locked out
            if (userIsLockedOut)
            {
                TempData["Message"] = "Cannot assign department to a locked out user. Please unlock the user before assigning roles.";
                return RedirectToAction(nameof(Index));
            }

            Guid? currentDepartmentId = user.DepartmentId;
            if (currentDepartmentId.HasValue && currentDepartmentId.Value.ToString() == departmentId) // Check if the current department is the same as the new value to avoid unnecessary updates
            {
                TempData["Message"] = "The user's department is already set to the specified value. No changes were made.";
                return RedirectToAction(nameof(Index));
            }

            user.DepartmentId = Guid.Parse(departmentId);
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Message"] = "User department has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "An error occurred while trying to update the user's department. Please try again.";
            return BadRequest(result.Errors);

        }

    }
}

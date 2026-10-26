
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly IUserRoleService _userRoleService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UserRolesController(
            IUserRoleService userRoleService,
            IUserService userService,
            IRoleService roleService)
        {
            _userRoleService = userRoleService;
            _userService = userService;
            _roleService = roleService;
        }

        public async Task<IActionResult> Index()
        {
            var userRoles = await _userRoleService.GetAllAsync();
            return View(userRoles);
        }

        public async Task<IActionResult> Details(long id)
        {
            var userRole = await _userRoleService.GetByIdAsync(id);

            if (userRole == null)
            {
                return NotFound();
            }

            return View(userRole);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View(new UserRoleDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(dto);
            }

            try
            {
                await _userRoleService.AssignAsync(dto);

                TempData["Success"] = "Role assigned to user successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to assign this role. Verify the user and role are active, belong to the same tenant, and the assignment does not already exist.");

                await LoadDropdownsAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var userRole = await _userRoleService.GetByIdAsync(id);

            if (userRole == null)
            {
                return NotFound();
            }

            return View(userRole);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var removed = await _userRoleService.RemoveAsync(id, null);

                if (!removed)
                {
                    return NotFound();
                }

                TempData["Success"] = "Role removed from user successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to remove the role from this user.";

                return RedirectToAction(nameof(Index));
            }
        }

        private async Task LoadDropdownsAsync()
        {
            var users = await _userService.GetAllAsync();
            var roles = await _roleService.GetAllAsync();

            ViewBag.Users = users
                .Where(user => user.IsActive)
                .Select(user => new SelectListItem
                {
                    Value = user.UserId.ToString(),
                    Text = $"{user.FirstName} {user.LastName} ({user.UserId})"
                })
                .ToList();

            ViewBag.Roles = roles
                .Where(role => role.IsActive)
                .Select(role => new SelectListItem
                {
                    Value = role.RoleId.ToString(),
                    Text = $"{role.RoleName} ({role.RoleCode})"
                })
                .ToList();
        }
    }
}

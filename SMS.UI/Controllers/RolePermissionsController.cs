
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class RolePermissionsController : Controller
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;

        public RolePermissionsController(
            IRolePermissionService rolePermissionService,
            IRoleService roleService,
            IPermissionService permissionService)
        {
            _rolePermissionService = rolePermissionService;
            _roleService = roleService;
            _permissionService = permissionService;
        }

        public async Task<IActionResult> Index()
        {
            var mappings = await _rolePermissionService.GetAllAsync();
            return View(mappings);
        }

        public async Task<IActionResult> Details(long id)
        {
            var mapping = await _rolePermissionService.GetByIdAsync(id);

            if (mapping == null)
            {
                return NotFound();
            }

            return View(mapping);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View(new RolePermissionDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolePermissionDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(dto);
            }

            try
            {
                await _rolePermissionService.AssignAsync(dto);
                TempData["Success"] =
                    "Permission assigned to the role successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to assign the permission. Verify that the role and permission are active and that this assignment does not already exist.");

                await LoadDropdownsAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var mapping = await _rolePermissionService.GetByIdAsync(id);

            if (mapping == null)
            {
                return NotFound();
            }

            return View(mapping);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var removed = await _rolePermissionService.RemoveAsync(id, null);

                if (!removed)
                {
                    return NotFound();
                }

                TempData["Success"] =
                    "Permission removed from the role successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to remove this role-permission assignment.";

                return RedirectToAction(nameof(Index));
            }
        }

        private async Task LoadDropdownsAsync()
        {
            var roles = await _roleService.GetAllAsync();
            var permissions = await _permissionService.GetAllAsync();

            ViewBag.Roles = roles
                .Where(role => role.IsActive)
                .Select(role => new SelectListItem
                {
                    Value = role.RoleId.ToString(),
                    Text = $"{role.RoleName} ({role.RoleCode})"
                })
                .ToList();

            ViewBag.Permissions = permissions
                .Where(permission => permission.IsActive)
                .Select(permission => new SelectListItem
                {
                    Value = permission.PermissionId.ToString(),
                    Text = $"{permission.PermissionName} ({permission.PermissionCode})"
                })
                .ToList();
        }
    }
}

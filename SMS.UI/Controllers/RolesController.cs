
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly ITenantService _tenantService;

        public RolesController(
            IRoleService roleService,
            ITenantService tenantService)
        {
            _roleService = roleService;
            _tenantService = tenantService;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllAsync();
            return View(roles);
        }

        public async Task<IActionResult> Details(long id)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        public async Task<IActionResult> Create()
        {
            await LoadTenantsAsync();
            return View(new RoleDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync();
                return View(dto);
            }

            try
            {
                await _roleService.CreateAsync(dto);
                TempData["Success"] = "Role created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the role. Check whether the role code already exists for this tenant.");

                await LoadTenantsAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            await LoadTenantsAsync();
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, RoleDto dto)
        {
            if (id != dto.RoleId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync();
                return View(dto);
            }

            try
            {
                var updated = await _roleService.UpdateAsync(dto);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["Success"] = "Role updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the role. Check whether the role code already exists for this tenant.");

                await LoadTenantsAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var deleted = await _roleService.DeleteAsync(id, null);

                if (!deleted)
                {
                    return NotFound();
                }

                TempData["Success"] = "Role deactivated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to deactivate this role.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task LoadTenantsAsync()
        {
            var tenants = await _tenantService.GetAllAsync();

            ViewBag.Tenants = tenants.Select(t => new SelectListItem
            {
                Value = t.TenantId.ToString(),
                Text = t.TenantName
            }).ToList();
        }
    }
}

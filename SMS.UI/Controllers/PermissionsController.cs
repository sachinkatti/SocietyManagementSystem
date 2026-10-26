
using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class PermissionsController : Controller
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<IActionResult> Index()
        {
            var permissions = await _permissionService.GetAllAsync();
            return View(permissions);
        }

        public async Task<IActionResult> Details(long id)
        {
            var permission = await _permissionService.GetByIdAsync(id);

            if (permission == null)
            {
                return NotFound();
            }

            return View(permission);
        }

        public IActionResult Create()
        {
            return View(new PermissionDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PermissionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _permissionService.CreateAsync(dto);
                TempData["Success"] = "Permission created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the permission. Check whether the permission code already exists.");

                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var permission = await _permissionService.GetByIdAsync(id);

            if (permission == null)
            {
                return NotFound();
            }

            return View(permission);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, PermissionDto dto)
        {
            if (id != dto.PermissionId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var updated = await _permissionService.UpdateAsync(dto);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["Success"] = "Permission updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the permission. Check whether the permission code already exists.");

                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var permission = await _permissionService.GetByIdAsync(id);

            if (permission == null)
            {
                return NotFound();
            }

            return View(permission);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var deleted = await _permissionService.DeleteAsync(id, null);

                if (!deleted)
                {
                    return NotFound();
                }

                TempData["Success"] = "Permission deactivated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to deactivate this permission.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

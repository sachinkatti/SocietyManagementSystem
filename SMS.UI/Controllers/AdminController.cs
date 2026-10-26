
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var admins = await _adminService.GetAllAsync();
            return View(admins);
        }

        public async Task<IActionResult> Details(long id)
        {
            var admin = await _adminService.GetByIdAsync(id);

            if (admin == null)
                return NotFound();

            return View(admin);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadAvailableUsersAsync();

            return View(new AdminDto
            {
                DateAssigned = DateTime.Today,
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminDto admin)
        {
            if (!ModelState.IsValid)
            {
                await LoadAvailableUsersAsync();
                return View(admin);
            }

            var adminId = await _adminService.CreateAsync(admin, null);

            if (adminId <= 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the admin.");

                await LoadAvailableUsersAsync();
                return View(admin);
            }

            TempData["SuccessMessage"] = "Admin created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var admin = await _adminService.GetByIdAsync(id);

            if (admin == null)
                return NotFound();

            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, AdminDto admin)
        {
            if (id != admin.AdminId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(admin);

            var updated = await _adminService.UpdateAsync(admin, null);

            if (!updated)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the admin.");

                return View(admin);
            }

            TempData["SuccessMessage"] = "Admin updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var admin = await _adminService.GetByIdAsync(id);

            if (admin == null)
                return NotFound();

            return View(admin);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _adminService.DeleteAsync(id, null);

            if (!deleted)
            {
                TempData["ErrorMessage"] = "Unable to delete the admin.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] =
                "Admin deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadAvailableUsersAsync()
        {
            var users = await _adminService.GetAvailableUsersForAdminAsync();

            ViewBag.AvailableUsers = users.Select(user =>
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = user.UserId.ToString(),
                    Text = $"{user.FirstName} {user.LastName} ({user.Email})"
                }).ToList();
        }
    }
}

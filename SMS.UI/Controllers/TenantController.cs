using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class TenantController : Controller
    {
        private readonly ITenantService _tenantService;

        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public async Task<IActionResult> Index()
        {
            var tenants = await _tenantService.GetAllAsync();
            return View(tenants);
        }

        public async Task<IActionResult> Details(long id)
        {
            var tenant = await _tenantService.GetByIdAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            return View(tenant);
        }

        public IActionResult Create()
        {
            return View(new TenantDto { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TenantDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _tenantService.CreateAsync(dto);

            TempData["SuccessMessage"] = "Tenant created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var tenant = await _tenantService.GetByIdAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            return View(tenant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, TenantDto dto)
        {
            if (id != dto.TenantId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var existingTenant = await _tenantService.GetByIdAsync(id);

            if (existingTenant == null)
            {
                return NotFound();
            }

            var updated = await _tenantService.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Tenant updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var tenant = await _tenantService.GetByIdAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            return View(tenant);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _tenantService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Tenant deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
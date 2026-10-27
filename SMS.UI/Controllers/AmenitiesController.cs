
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class AmenitiesController : Controller
    {
        private readonly IAmenityService _amenityService;
        private readonly ITenantService _tenantService;

        public AmenitiesController(
            IAmenityService amenityService,
            ITenantService tenantService)
        {
            _amenityService = amenityService;
            _tenantService = tenantService;
        }

        public async Task<IActionResult> Index()
        {
            var amenities = await _amenityService.GetAllAsync();
            return View(amenities);
        }

        public async Task<IActionResult> Details(long id)
        {
            var amenity = await _amenityService.GetByIdAsync(id);

            if (amenity == null)
                return NotFound();

            return View(amenity);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadTenantsAsync();
            return View(new AmenityDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AmenityDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync(dto.TenantId);
                return View(dto);
            }

            try
            {
                await _amenityService.CreateAsync(dto);

                TempData["SuccessMessage"] = "Amenity created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex) when (
                ex.Number == 50001 ||
                ex.Number == 547)
            {
                ModelState.AddModelError(
                    nameof(dto.TenantId),
                    "The selected tenant is invalid. Please select a valid tenant.");

                await LoadTenantsAsync(dto.TenantId);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var amenity = await _amenityService.GetByIdAsync(id);

            if (amenity == null)
                return NotFound();

            await LoadTenantsAsync(amenity.TenantId);
            return View(amenity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, AmenityDto dto)
        {
            if (id != dto.AmenityId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync(dto.TenantId);
                return View(dto);
            }

            try
            {
                bool updated = await _amenityService.UpdateAsync(dto);

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] = "Amenity updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex) when (
                ex.Number == 50001 ||
                ex.Number == 547)
            {
                ModelState.AddModelError(
                    nameof(dto.TenantId),
                    "The selected tenant is invalid. Please select a valid tenant.");

                await LoadTenantsAsync(dto.TenantId);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var amenity = await _amenityService.GetByIdAsync(id);

            if (amenity == null)
                return NotFound();

            return View(amenity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            bool deleted = await _amenityService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["SuccessMessage"] = "Amenity deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadTenantsAsync(long? selectedTenantId = null)
        {
            var tenants = await _tenantService.GetAllAsync();

            ViewBag.Tenants = new SelectList(
                tenants.Where(t => t.IsActive),
                "TenantId",
                "TenantName",
                selectedTenantId);
        }
    }
}

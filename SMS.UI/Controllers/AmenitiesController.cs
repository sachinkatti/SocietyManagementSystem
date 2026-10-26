
using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class AmenitiesController : Controller
    {
        private readonly IAmenityService _amenityService;

        public AmenitiesController(IAmenityService amenityService)
        {
            _amenityService = amenityService;
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
        public IActionResult Create()
        {
            return View(new AmenityDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AmenityDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                ModelState.AddModelError(nameof(dto.Name), "Name is required.");

            if (dto.DepositRequired < 0)
                ModelState.AddModelError(nameof(dto.DepositRequired), "Deposit cannot be negative.");

            if (dto.MaxSlotHours <= 0)
                ModelState.AddModelError(nameof(dto.MaxSlotHours), "Maximum slot hours must be greater than zero.");

            if (dto.TenantId <= 0)
                ModelState.AddModelError(nameof(dto.TenantId), "Enter a valid Tenant ID.");

            if (!ModelState.IsValid)
                return View(dto);

            await _amenityService.CreateAsync(dto);

            TempData["Success"] = "Amenity created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var amenity = await _amenityService.GetByIdAsync(id);

            if (amenity == null)
                return NotFound();

            return View(amenity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, AmenityDto dto)
        {
            if (id != dto.AmenityId)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(dto.Name))
                ModelState.AddModelError(nameof(dto.Name), "Name is required.");

            if (dto.DepositRequired < 0)
                ModelState.AddModelError(nameof(dto.DepositRequired), "Deposit cannot be negative.");

            if (dto.MaxSlotHours <= 0)
                ModelState.AddModelError(nameof(dto.MaxSlotHours), "Maximum slot hours must be greater than zero.");

            if (dto.TenantId <= 0)
                ModelState.AddModelError(nameof(dto.TenantId), "Enter a valid Tenant ID.");

            if (!ModelState.IsValid)
                return View(dto);

            var updated = await _amenityService.UpdateAsync(dto);

            if (!updated)
                return NotFound();

            TempData["Success"] = "Amenity updated successfully.";
            return RedirectToAction(nameof(Index));
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
            var deleted = await _amenityService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["Success"] = "Amenity deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}

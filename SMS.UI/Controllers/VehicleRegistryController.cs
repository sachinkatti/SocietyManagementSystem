using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class VehicleRegistryController : Controller
    {
        private readonly IVehicleRegistryService _vehicleService;
        private readonly ITenantService _tenantService;

        public VehicleRegistryController(
            IVehicleRegistryService vehicleService,
            ITenantService tenantService)
        {
            _vehicleService = vehicleService;
            _tenantService = tenantService;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _vehicleService.GetAllAsync();
            return View(vehicles);
        }

        public async Task<IActionResult> Details(long id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);

            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        public async Task<IActionResult> Create()
        {
            await LoadTenantsAsync();
            ViewBag.Flats = new SelectList(
                Enumerable.Empty<SelectListItem>(), "Value", "Text");

            return View(new VehicleRegistryDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleRegistryDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadFormDropdownsAsync(dto.TenantId, dto.FlatId);
                return View(dto);
            }

            try
            {
                await _vehicleService.CreateAsync(dto);
                TempData["Success"] = "Vehicle registered successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex) when (
                ex.Number == 2627 || ex.Number == 2601)
            {
                ModelState.AddModelError(
                    nameof(dto.VehicleNumber),
                    "This vehicle number is already registered.");

                await LoadFormDropdownsAsync(dto.TenantId, dto.FlatId);
                return View(dto);
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                ModelState.AddModelError(
                    nameof(dto.FlatId),
                    "Please select an active flat belonging to the selected tenant.");

                await LoadFormDropdownsAsync(dto.TenantId, dto.FlatId);
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);

            if (vehicle == null)
                return NotFound();

            await LoadFormDropdownsAsync(vehicle.TenantId, vehicle.FlatId);
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VehicleRegistryDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadFormDropdownsAsync(dto.TenantId, dto.FlatId);
                return View(dto);
            }

            try
            {
                var updated = await _vehicleService.UpdateAsync(dto);

                if (!updated)
                    return NotFound();

                TempData["Success"] = "Vehicle updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex) when (
                ex.Number == 2627 || ex.Number == 2601)
            {
                ModelState.AddModelError(
                    nameof(dto.VehicleNumber),
                    "This vehicle number is already registered.");

                await LoadFormDropdownsAsync(dto.TenantId, dto.FlatId);
                return View(dto);
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                ModelState.AddModelError(
                    nameof(dto.FlatId),
                    "Please select an active flat belonging to the selected tenant.");

                await LoadFormDropdownsAsync(dto.TenantId, dto.FlatId);
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);

            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _vehicleService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["Success"] = "Vehicle deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetFlatsByTenant(long tenantId)
        {
            if (tenantId <= 0)
                return Json(Array.Empty<object>());

            var flats = await _vehicleService.GetFlatsByTenantAsync(tenantId);

            var result = flats.Select(f => new
            {
                id = f.FlatId,
                text = $"{f.WingName} - {f.FlatNumber}"
            });

            return Json(result);
        }

        private async Task LoadTenantsAsync()
        {
            var tenants = await _tenantService.GetAllAsync();

            ViewBag.Tenants = new SelectList(
                tenants.Where(t => t.IsActive),
                "TenantId",
                "TenantName");
        }

        private async Task LoadFormDropdownsAsync(
            long tenantId,
            long? selectedFlatId = null)
        {
            await LoadTenantsAsync();

            var flats = tenantId > 0
                ? await _vehicleService.GetFlatsByTenantAsync(tenantId)
                : Enumerable.Empty<SMS.Infrastructure.VehicleFlatOption>();

            ViewBag.Flats = new SelectList(
                flats.Select(f => new
                {
                    f.FlatId,
                    DisplayName = $"{f.WingName} - {f.FlatNumber}"
                }),
                "FlatId",
                "DisplayName",
                selectedFlatId);
        }
    }
}
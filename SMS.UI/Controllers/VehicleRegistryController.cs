using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class VehicleRegistryController : Controller
    {
        private readonly IVehicleRegistryService _service;
        private readonly IFlatService _flatService;

        public VehicleRegistryController(
            IVehicleRegistryService service,
            IFlatService flatService)
        {
            _service = service;
            _flatService = flatService;
        }

        private async Task LoadFlatsAsync(long? selectedId = null)
        {
            var flats = await _flatService.GetAllAsync();

            ViewBag.Flats = new SelectList(
                flats,
                "FlatId",
                "FlatNumber",
                selectedId);
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = (await _service.GetAllAsync()).ToList();
            var flats = await _flatService.GetAllAsync();
            var lookup = flats.ToDictionary(f => f.FlatId, f => f.FlatNumber);

            foreach (var vehicle in vehicles)
            {
                if (lookup.TryGetValue(vehicle.FlatId, out var number))
                    vehicle.FlatNumber = number;
            }

            return View(vehicles);
        }

        public async Task<IActionResult> Details(long id)
        {
            var vehicle = await _service.GetByIdAsync(id);
            if (vehicle == null) return NotFound();

            await LoadFlatsAsync(vehicle.FlatId);
            return View(vehicle);
        }

        public async Task<IActionResult> Create()
        {
            await LoadFlatsAsync();
            return View(new VehicleRegistryDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleRegistryDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadFlatsAsync(dto.FlatId);
                return View(dto);
            }

            await _service.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var vehicle = await _service.GetByIdAsync(id);
            if (vehicle == null) return NotFound();

            await LoadFlatsAsync(vehicle.FlatId);
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, VehicleRegistryDto dto)
        {
            if (id != dto.VehicleId) return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadFlatsAsync(dto.FlatId);
                return View(dto);
            }

            if (!await _service.UpdateAsync(dto)) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var vehicle = await _service.GetByIdAsync(id);
            if (vehicle == null) return NotFound();

            var flats = await _flatService.GetAllAsync();
            vehicle.FlatNumber = flats
                .FirstOrDefault(f => f.FlatId == vehicle.FlatId)?.FlatNumber ?? "";

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
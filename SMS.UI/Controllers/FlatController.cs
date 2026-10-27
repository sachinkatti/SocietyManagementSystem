
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class FlatController : Controller
    {
        private readonly IFlatService _flatService;
        private readonly IWingService _wingService;

        public FlatController(
            IFlatService flatService,
            IWingService wingService)
        {
            _flatService = flatService;
            _wingService = wingService;
        }

        // GET: Flat
        public async Task<IActionResult> Index()
        {
            var flats = await _flatService.GetAllAsync();
            return View(flats);
        }

        // GET: Flat/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var flat = await _flatService.GetByIdAsync(id);

            if (flat == null)
                return NotFound();

            return View(flat);
        }

        // GET: Flat/Create
        public async Task<IActionResult> Create()
        {
            await LoadWingDropdownAsync();
            return View();
        }

        // POST: Flat/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlatDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadWingDropdownAsync(dto.WingId);
                return View(dto);
            }

            await _flatService.CreateAsync(dto);

            TempData["SuccessMessage"] = "Flat created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Flat/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var flat = await _flatService.GetByIdAsync(id);

            if (flat == null)
                return NotFound();

            await LoadWingDropdownAsync(flat.WingId);

            return View(flat);
        }

        // POST: Flat/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FlatDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadWingDropdownAsync(dto.WingId);
                return View(dto);
            }

            var updated = await _flatService.UpdateAsync(dto);

            if (!updated)
                return NotFound();

            TempData["SuccessMessage"] = "Flat updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Flat/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var flat = await _flatService.GetByIdAsync(id);

            if (flat == null)
                return NotFound();

            return View(flat);
        }

        // POST: Flat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _flatService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["SuccessMessage"] = "Flat deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadWingDropdownAsync(long? selectedWingId = null)
        {
            var wings = await _wingService.GetAllAsync();

            ViewBag.Wings = wings.Select(w => new SelectListItem
            {
                Value = w.WingId.ToString(),
                Text = w.WingName,
                Selected = selectedWingId.HasValue &&
                           selectedWingId.Value == w.WingId
            }).ToList();
        }
    }
}

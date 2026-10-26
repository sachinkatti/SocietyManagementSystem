
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class FlatsController : Controller
    {
        private readonly IFlatService _flatService;

        public FlatsController(IFlatService flatService)
        {
            _flatService = flatService;
        }

        public async Task<IActionResult> Index()
        {
            var flats = await _flatService.GetAllAsync();
            return View(flats);
        }

        public IActionResult Create()
        {
            return View(new FlatDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlatDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _flatService.CreateAsync(dto);
                TempData["Success"] = "Flat created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                ModelState.AddModelError(
                    nameof(dto.WingId),
                    "The selected Wing ID does not exist.");
            }
            catch (SqlException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the flat. Please verify the database values.");
            }

            return View(dto);
        }

        public async Task<IActionResult> Details(long id)
        {
            var flat = await _flatService.GetByIdAsync(id);

            if (flat == null)
            {
                return NotFound();
            }

            return View(flat);
        }

        public async Task<IActionResult> Edit(long id)
        {
            var flat = await _flatService.GetByIdAsync(id);

            if (flat == null)
            {
                return NotFound();
            }

            return View(flat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, FlatDto dto)
        {
            if (id != dto.FlatId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var updated = await _flatService.UpdateAsync(dto);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["Success"] = "Flat updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                ModelState.AddModelError(
                    nameof(dto.WingId),
                    "The selected Wing ID does not exist.");
            }
            catch (SqlException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the flat. Please verify the database values.");
            }

            return View(dto);
        }

        public async Task<IActionResult> Delete(long id)
        {
            var flat = await _flatService.GetByIdAsync(id);

            if (flat == null)
            {
                return NotFound();
            }

            return View(flat);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _flatService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["Success"] = "Flat deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
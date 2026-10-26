
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class ResidentsController : Controller
    {
        private readonly IResidentService _residentService;

        public ResidentsController(IResidentService residentService)
        {
            _residentService = residentService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _residentService.GetAllAsync());
        }

        public async Task<IActionResult> Details(long id)
        {
            var resident = await _residentService.GetByIdAsync(id);

            if (resident == null)
            {
                return NotFound();
            }

            return View(resident);
        }

        public IActionResult Create()
        {
            return View(new ResidentDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResidentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _residentService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Resident created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException)
            {
                ModelState.AddModelError(string.Empty,
                    "Unable to create resident. Verify that the User ID and Flat ID exist.");
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var resident = await _residentService.GetByIdAsync(id);

            if (resident == null)
            {
                return NotFound();
            }

            return View(resident);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ResidentDto dto)
        {
            if (id != dto.ResidentId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var updated = await _residentService.UpdateAsync(dto);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Resident updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException)
            {
                ModelState.AddModelError(string.Empty,
                    "Unable to update resident. Verify that the User ID and Flat ID exist.");
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var resident = await _residentService.GetByIdAsync(id);

            if (resident == null)
            {
                return NotFound();
            }

            return View(resident);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _residentService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Resident deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}

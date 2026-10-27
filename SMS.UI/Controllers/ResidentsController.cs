
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class ResidentsController : Controller
    {
        private readonly IResidentService _residentService;
        private readonly IUserService _userService;
        private readonly IFlatService _flatService;

        public ResidentsController(
            IResidentService residentService,
            IUserService userService,
            IFlatService flatService)
        {
            _residentService = residentService;
            _userService = userService;
            _flatService = flatService;
        }

        public async Task<IActionResult> Index()
        {
            var residents = await _residentService.GetAllAsync();
            return View(residents);
        }

        public async Task<IActionResult> Details(long id)
        {
            var resident = await _residentService.GetByIdAsync(id);

            if (resident == null)
                return NotFound();

            return View(resident);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();

            return View(new ResidentDto
            {
                MoveInDate = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResidentDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(dto.UserId, dto.FlatId);
                return View(dto);
            }

            await _residentService.CreateAsync(dto);

            TempData["SuccessMessage"] = "Resident created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var resident = await _residentService.GetByIdAsync(id);

            if (resident == null)
                return NotFound();

            await LoadDropdownsAsync(resident.UserId, resident.FlatId);

            return View(resident);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ResidentDto dto)
        {
            if (id != dto.ResidentId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(dto.UserId, dto.FlatId);
                return View(dto);
            }

            var existing = await _residentService.GetByIdAsync(id);

            if (existing == null)
                return NotFound();

            var updated = await _residentService.UpdateAsync(dto);

            if (!updated)
                return NotFound();

            TempData["SuccessMessage"] = "Resident updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var resident = await _residentService.GetByIdAsync(id);

            if (resident == null)
                return NotFound();

            return View(resident);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _residentService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["SuccessMessage"] = "Resident deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync(
            long? selectedUserId = null,
            long? selectedFlatId = null)
        {
            var users = await _userService.GetAllAsync();

            ViewBag.Users = new SelectList(
                users.Where(u => u.IsActive),
                "UserId",
                "FullName",
                selectedUserId);

            var flats = await _flatService.GetAllAsync();

            ViewBag.Flats = new SelectList(
                flats.Where(f => f.IsActive),
                "FlatId",
                "FlatNumber",
                selectedFlatId);
        }
    }
}
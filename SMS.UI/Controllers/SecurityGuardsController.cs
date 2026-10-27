
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class SecurityGuardsController : Controller
    {
        private readonly ISecurityGuardService _guardService;
        private readonly IUserService _userService;

        public SecurityGuardsController(
            ISecurityGuardService guardService,
            IUserService userService)
        {
            _guardService = guardService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var guards = await _guardService.GetAllAsync();
            return View(guards);
        }

        public async Task<IActionResult> Details(long id)
        {
            var guard = await _guardService.GetByIdAsync(id);

            if (guard == null)
                return NotFound();

            return View(guard);
        }

        public async Task<IActionResult> Create()
        {
            await LoadUsersAsync();
            return View(new SecurityGuardDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SecurityGuardDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadUsersAsync(dto.UserId);
                return View(dto);
            }

            try
            {
                await _guardService.CreateAsync(dto);
                TempData["Success"] = "Security guard created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
                when (ex.Number == 2627 || ex.Number == 2601)
            {
                ModelState.AddModelError(
                    nameof(dto.UserId),
                    "This user is already registered as a security guard.");

                await LoadUsersAsync(dto.UserId);
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var guard = await _guardService.GetByIdAsync(id);

            if (guard == null)
                return NotFound();

            await LoadUsersAsync(guard.UserId);

            return View(guard);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SecurityGuardDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadUsersAsync(dto.UserId);
                return View(dto);
            }

            try
            {
                var updated = await _guardService.UpdateAsync(dto);

                if (!updated)
                    return NotFound();

                TempData["Success"] = "Security guard updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
                when (ex.Number == 2627 || ex.Number == 2601)
            {
                ModelState.AddModelError(
                    nameof(dto.UserId),
                    "This user is already registered as a security guard.");

                await LoadUsersAsync(dto.UserId);
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var guard = await _guardService.GetByIdAsync(id);

            if (guard == null)
                return NotFound();

            return View(guard);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _guardService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["Success"] = "Security guard deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadUsersAsync(long? selectedUserId = null)
        {
            var users = (await _userService.GetAllAsync())
                .Where(u => u.IsActive)
                .ToList();

            var guards = await _guardService.GetAllAsync();
            var assignedUserIds = guards
                .Select(g => g.UserId)
                .Where(userId => userId != selectedUserId)
                .ToHashSet();

            ViewBag.Users = new SelectList(
                users.Where(u => !assignedUserIds.Contains(u.UserId)),
                "UserId",
                "FullName",
                selectedUserId);
        }
    }
}
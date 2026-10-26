
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class SecurityGuardsController : Controller
    {
        private readonly ISecurityGuardService _securityGuardService;
        private readonly IUserService _userService;

        public SecurityGuardsController(
            ISecurityGuardService securityGuardService,
            IUserService userService)
        {
            _securityGuardService = securityGuardService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var guards = (await _securityGuardService.GetAllAsync()).ToList();
            var users = await _userService.GetActiveUsersForDropdownAsync();

            var userLookup = users.ToDictionary(u => u.UserId);

            foreach (var guard in guards)
            {
                if (userLookup.TryGetValue(guard.UserId, out var user))
                {
                    guard.UserFullName =
                        $"{user.FirstName} {user.LastName}";

                    guard.UserEmail = user.Email;
                }
                else
                {
                    guard.UserFullName = "User not found";
                }
            }

            return View(guards);
        }

        public async Task<IActionResult> Details(long id)
        {
            var guard = await _securityGuardService.GetByIdAsync(id);

            if (guard == null)
            {
                return NotFound();
            }

            await LoadUserDetailsAsync(guard);
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
                await _securityGuardService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Security guard created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException)
            {
                ModelState.AddModelError(string.Empty,
                    "Unable to create security guard. Verify the selected user exists.");
                await LoadUsersAsync(dto.UserId);
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var guard = await _securityGuardService.GetByIdAsync(id);

            if (guard == null)
            {
                return NotFound();
            }

            await LoadUsersAsync(guard.UserId);
            return View(guard);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, SecurityGuardDto dto)
        {
            if (id != dto.GuardId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadUsersAsync(dto.UserId);
                return View(dto);
            }

            try
            {
                var updated = await _securityGuardService.UpdateAsync(dto);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Security guard updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException)
            {
                ModelState.AddModelError(string.Empty,
                    "Unable to update security guard. Verify the selected user exists.");
                await LoadUsersAsync(dto.UserId);
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var guard = await _securityGuardService.GetByIdAsync(id);

            if (guard == null)
            {
                return NotFound();
            }

            await LoadUserDetailsAsync(guard);
            return View(guard);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _securityGuardService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Security guard deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadUsersAsync(long? selectedUserId = null)
        {
            var users = await _userService.GetActiveUsersForDropdownAsync();

            ViewBag.Users = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                users.Select(user => new
                {
                    user.UserId,
                    DisplayName = $"{user.FirstName} {user.MiddleName} {user.LastName} ({user.Email})"
                        .Replace("  ", " ")
                        .Trim()
                }),
                "UserId",
                "DisplayName",
                selectedUserId
            );
        }

        private async Task LoadUserDetailsAsync(SecurityGuardDto guard)
        {
            var users = await _userService.GetActiveUsersForDropdownAsync();
            var user = users.FirstOrDefault(item => item.UserId == guard.UserId);

            if (user != null)
            {
                guard.UserFullName = $"{user.FirstName} {user.MiddleName} {user.LastName}"
                    .Replace("  ", " ")
                    .Trim();

                guard.UserEmail = user.Email;
            }
        }
    }
}

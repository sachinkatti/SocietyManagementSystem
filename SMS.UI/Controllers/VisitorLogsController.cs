using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class VisitorLogsController : Controller
    {
        private readonly IVisitorLogService _visitorLogService;
        private readonly IFlatService _flatService;
        private readonly ISecurityGuardService _securityGuardService;
        private readonly IUserService _userService;

        public VisitorLogsController(
            IVisitorLogService visitorLogService,
            IFlatService flatService,
            ISecurityGuardService securityGuardService,
            IUserService userService)
        {
            _visitorLogService = visitorLogService;
            _flatService = flatService;
            _securityGuardService = securityGuardService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _visitorLogService.GetAllAsync();
            var flats = await _flatService.GetAllAsync();
            var guards = await _securityGuardService.GetAllAsync();
            var users = await _userService.GetActiveUsersForDropdownAsync();

            var flatLookup = flats.ToDictionary(f => f.FlatId, f => f.FlatNumber);
            var guardLookup = guards.ToDictionary(g => g.GuardId, g => g.UserId);
            var userLookup = users.ToDictionary(u => u.UserId);

            foreach (var log in logs)
            {
                log.FlatNumber = flatLookup.TryGetValue(log.VisitingFlatId, out var flatNumber)
                    ? flatNumber : "Unknown";

                if (log.LoggedByGuardId.HasValue &&
                    guardLookup.TryGetValue(log.LoggedByGuardId.Value, out var userId) &&
                    userLookup.TryGetValue(userId, out var user))
                {
                    log.GuardName = $"{user.FirstName} {user.LastName}";
                }
                else
                {
                    log.GuardName = "Not assigned";
                }
            }

            return View(logs);
        }

        public async Task<IActionResult> Details(long id)
        {
            var dto = await _visitorLogService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            await PopulateDisplayNames(dto);
            return View(dto);
        }

        public async Task<IActionResult> Create()
        {
            var dto = new VisitorLogDto
            {
                EntryTime = DateTime.Now,
                ApprovalMethod = "Guard"
            };

            await PopulateDropdowns();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitorLogDto dto)
        {
            ValidateVisitorLog(dto);

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(dto.VisitingFlatId, dto.LoggedByGuardId);
                return View(dto);
            }

            dto.IsActive = true;
            await _visitorLogService.CreateAsync(dto);

            TempData["SuccessMessage"] = "Visitor log created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var dto = await _visitorLogService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            await PopulateDropdowns(dto.VisitingFlatId, dto.LoggedByGuardId);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, VisitorLogDto dto)
        {
            if (id != dto.VisitorId)
                return BadRequest();

            ValidateVisitorLog(dto);

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(dto.VisitingFlatId, dto.LoggedByGuardId);
                return View(dto);
            }

            var updated = await _visitorLogService.UpdateAsync(dto);

            if (!updated)
                return NotFound();

            TempData["SuccessMessage"] = "Visitor log updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var dto = await _visitorLogService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            await PopulateDisplayNames(dto);
            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _visitorLogService.DeleteAsync(id, null);

            if (!deleted)
                return NotFound();

            TempData["SuccessMessage"] = "Visitor log deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void ValidateVisitorLog(VisitorLogDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.VisitorName))
                ModelState.AddModelError(nameof(dto.VisitorName), "Visitor name is required.");
            else if (dto.VisitorName.Length > 100)
                ModelState.AddModelError(nameof(dto.VisitorName), "Visitor name cannot exceed 100 characters.");

            if (dto.PhoneNumber?.Length > 15)
                ModelState.AddModelError(nameof(dto.PhoneNumber), "Phone number cannot exceed 15 characters.");

            if (dto.VisitingFlatId <= 0)
                ModelState.AddModelError(nameof(dto.VisitingFlatId), "Please select a flat.");

            if (string.IsNullOrWhiteSpace(dto.Purpose))
                ModelState.AddModelError(nameof(dto.Purpose), "Purpose is required.");
            else if (dto.Purpose.Length > 50)
                ModelState.AddModelError(nameof(dto.Purpose), "Purpose cannot exceed 50 characters.");

            if (dto.VehicleNumber?.Length > 20)
                ModelState.AddModelError(nameof(dto.VehicleNumber), "Vehicle number cannot exceed 20 characters.");

            if (string.IsNullOrWhiteSpace(dto.ApprovalMethod))
                ModelState.AddModelError(nameof(dto.ApprovalMethod), "Approval method is required.");
            else if (dto.ApprovalMethod.Length > 20)
                ModelState.AddModelError(nameof(dto.ApprovalMethod), "Approval method cannot exceed 20 characters.");

            if (dto.EntryTime == default)
                ModelState.AddModelError(nameof(dto.EntryTime), "Entry time is required.");

            if (dto.ExitTime.HasValue && dto.ExitTime.Value < dto.EntryTime)
                ModelState.AddModelError(nameof(dto.ExitTime), "Exit time cannot be earlier than entry time.");

            if (dto.Description?.Length > 500)
                ModelState.AddModelError(nameof(dto.Description), "Description cannot exceed 500 characters.");
        }

        private async Task PopulateDropdowns(long? selectedFlatId = null, long? selectedGuardId = null)
        {
            var flats = await _flatService.GetAllAsync();
            var guards = await _securityGuardService.GetAllAsync();
            var users = await _userService.GetActiveUsersForDropdownAsync();

            ViewBag.Flats = new SelectList(
                flats, "FlatId", "FlatNumber", selectedFlatId);

            var userLookup = users.ToDictionary(u => u.UserId);

            ViewBag.Guards = guards.Select(g => new SelectListItem
            {
                Value = g.GuardId.ToString(),
                Text = userLookup.TryGetValue(g.UserId, out var user)
                    ? $"{user.FirstName} {user.LastName}"
                    : $"Guard {g.GuardId}",
                Selected = selectedGuardId == g.GuardId
            }).ToList();
        }

        private async Task PopulateDisplayNames(VisitorLogDto dto)
        {
            var flats = await _flatService.GetAllAsync();
            var guards = await _securityGuardService.GetAllAsync();
            var users = await _userService.GetActiveUsersForDropdownAsync();

            dto.FlatNumber = flats.FirstOrDefault(f => f.FlatId == dto.VisitingFlatId)?.FlatNumber;

            var guard = dto.LoggedByGuardId.HasValue
                ? guards.FirstOrDefault(g => g.GuardId == dto.LoggedByGuardId.Value)
                : null;

            var user = guard == null ? null : users.FirstOrDefault(u => u.UserId == guard.UserId);

            dto.GuardName = user == null
                ? "Not assigned"
                : $"{user.FirstName} {user.LastName}";
        }
    }
}
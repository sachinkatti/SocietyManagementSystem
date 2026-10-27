
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class VisitorLogsController : Controller
    {
        private readonly IVisitorLogService _visitorLogService;
        private readonly ITenantService _tenantService;

        public VisitorLogsController(
            IVisitorLogService visitorLogService,
            ITenantService tenantService)
        {
            _visitorLogService = visitorLogService;
            _tenantService = tenantService;
        }

        // 1. Display all visitor logs
        public async Task<IActionResult> Index()
        {
            var visitorLogs = await _visitorLogService.GetAllAsync();
            return View(visitorLogs);
        }

        // 2. Display visitor log details
        public async Task<IActionResult> Details(long id)
        {
            var visitorLog = await _visitorLogService.GetByIdAsync(id);

            if (visitorLog == null)
                return NotFound();

            return View(visitorLog);
        }

        // 3. Open create form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadTenantsAsync();

            return View(new VisitorLogDto
            {
                EntryTime = DateTime.Now
            });
        }

        // 4. Save new visitor log
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitorLogDto visitorLog)
        {
            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync();
                return View(visitorLog);
            }

            try
            {
                var visitorId = await _visitorLogService.CreateAsync(visitorLog);

                TempData["SuccessMessage"] =
                    $"Visitor log created successfully. ID: {visitorId}";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty, ex.Message);

                await LoadTenantsAsync();
                return View(visitorLog);
            }
        }

        // 5. Open edit form
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var visitorLog = await _visitorLogService.GetByIdAsync(id);

            if (visitorLog == null)
                return NotFound();

            await LoadTenantsAsync();

            return View(visitorLog);
        }

        // 6. Update visitor log
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            long id, VisitorLogDto visitorLog)
        {
            if (id != visitorLog.VisitorId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync();
                return View(visitorLog);
            }

            try
            {
                var updated = await _visitorLogService.UpdateAsync(visitorLog);

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] =
                    "Visitor log updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty, ex.Message);

                await LoadTenantsAsync();
                return View(visitorLog);
            }
        }

        // 7. Open delete confirmation
        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var visitorLog = await _visitorLogService.GetByIdAsync(id);

            if (visitorLog == null)
                return NotFound();

            return View(visitorLog);
        }

        // 8. Soft-delete visitor log
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _visitorLogService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["SuccessMessage"] =
                "Visitor log deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // 9. AJAX: get active flats for selected tenant
        [HttpGet]
        public async Task<IActionResult> GetFlatsByTenant(long tenantId)
        {
            if (tenantId <= 0)
                return Json(Array.Empty<object>());

            var flats = await _visitorLogService
                .GetFlatsByTenantAsync(tenantId);

            return Json(flats.Select(f => new
            {
                id = f.FlatId,
                text = $"{f.FlatNumber} - {f.WingName}"
            }));
        }

        // 10. AJAX: get active guards for selected tenant
        [HttpGet]
        public async Task<IActionResult> GetGuardsByTenant(long tenantId)
        {
            if (tenantId <= 0)
                return Json(Array.Empty<object>());

            var guards = await _visitorLogService
                .GetGuardsByTenantAsync(tenantId);

            return Json(guards.Select(g => new
            {
                id = g.GuardId,
                text = g.GuardName
            }));
        }

        // Load tenant dropdown
        private async Task LoadTenantsAsync()
        {
            var tenants = await _tenantService.GetAllAsync();

            ViewBag.Tenants = new SelectList(
                tenants,
                "TenantId",
                "TenantName");
        }
    }
}

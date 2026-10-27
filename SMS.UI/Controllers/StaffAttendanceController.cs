
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class StaffAttendanceController : Controller
    {
        private readonly IStaffAttendanceService _attendanceService;
        private readonly ITenantService _tenantService;

        public StaffAttendanceController(
            IStaffAttendanceService attendanceService,
            ITenantService tenantService)
        {
            _attendanceService = attendanceService;
            _tenantService = tenantService;
        }

        // 1. List attendance records
        public async Task<IActionResult> Index()
        {
            var attendanceList = await _attendanceService.GetAllAsync();
            return View(attendanceList);
        }

        // 2. Display create form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadTenantsAsync();

            return View(new StaffAttendanceDto
            {
                CheckInTime = DateTime.Now
            });
        }

        // 3. Save new attendance record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffAttendanceDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync();
                await LoadFlatsAsync(dto.TenantId);
                return View(dto);
            }

            try
            {
                await _attendanceService.CreateAsync(dto);

                TempData["SuccessMessage"] =
                    "Staff attendance created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadTenantsAsync();
                await LoadFlatsAsync(dto.TenantId);
                return View(dto);
            }
        }

        // 4. Display edit form
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var dto = await _attendanceService.GetByIdAsync(id);

            if (dto == null)
            {
                return NotFound();
            }

            await LoadTenantsAsync();
            await LoadFlatsAsync(dto.TenantId);

            return View(dto);
        }

        // 5. Save edited attendance record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            long id, StaffAttendanceDto dto)
        {
            if (id != dto.AttendanceId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync();
                await LoadFlatsAsync(dto.TenantId);
                return View(dto);
            }

            try
            {
                var updated = await _attendanceService.UpdateAsync(dto);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] =
                    "Staff attendance updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadTenantsAsync();
                await LoadFlatsAsync(dto.TenantId);
                return View(dto);
            }
        }

        // 6. Display details
        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var dto = await _attendanceService.GetByIdAsync(id);

            if (dto == null)
            {
                return NotFound();
            }

            return View(dto);
        }

        // 7. Display delete confirmation
        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var dto = await _attendanceService.GetByIdAsync(id);

            if (dto == null)
            {
                return NotFound();
            }

            return View(dto);
        }

        // 8. Confirm soft delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            long id, long? modifyBy)
        {
            var deleted = await _attendanceService.DeleteAsync(id, modifyBy);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] =
                "Staff attendance deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // 9. Load flats when the society changes
        [HttpGet]
        public async Task<IActionResult> GetFlatsByTenant(long tenantId)
        {
            if (tenantId <= 0)
            {
                return Json(new object[0]);
            }

            var flats = await _attendanceService.GetFlatsByTenantAsync(tenantId);

            var result = flats.Select(f => new
            {
                flatId = f.FlatId,
                flatNumber = f.FlatNumber,
                wingName = f.WingName
            });

            return Json(result);
        }

        // Helper: load society dropdown
        private async Task LoadTenantsAsync()
        {
            var tenants = await _tenantService.GetAllAsync();

            ViewBag.Tenants = new SelectList(
                tenants,
                "TenantId",
                "TenantName");
        }

        // Helper: load flat dropdown
        private async Task LoadFlatsAsync(long tenantId)
        {
            var flats = tenantId > 0
                ? await _attendanceService.GetFlatsByTenantAsync(tenantId)
                : new List<SMS.Domain.StaffAttendance>();

            ViewBag.Flats = new SelectList(
                flats.Select(f => new
                {
                    f.FlatId,
                    DisplayName = $"{f.WingName} - {f.FlatNumber}"
                }),
                "FlatId",
                "DisplayName");
        }
    }
}
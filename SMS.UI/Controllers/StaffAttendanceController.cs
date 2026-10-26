using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class StaffAttendanceController : Controller
    {
        private readonly IStaffAttendanceService _service;

        public StaffAttendanceController(IStaffAttendanceService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var records = await _service.GetAllAsync();
            return View(records);
        }

        public async Task<IActionResult> Details(long id)
        {
            var record = await _service.GetByIdAsync(id);

            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new StaffAttendanceDto
            {
                CheckInTime = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffAttendanceDto dto)
        {
            ValidateAttendance(dto);

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _service.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var record = await _service.GetByIdAsync(id);

            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, StaffAttendanceDto dto)
        {
            if (id != dto.AttendanceId)
            {
                return BadRequest();
            }

            ValidateAttendance(dto);

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var updated = await _service.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var record = await _service.GetByIdAsync(id);

            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private void ValidateAttendance(StaffAttendanceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.StaffName))
            {
                ModelState.AddModelError(nameof(dto.StaffName), "Staff name is required.");
            }
            else if (dto.StaffName.Trim().Length > 100)
            {
                ModelState.AddModelError(nameof(dto.StaffName), "Staff name cannot exceed 100 characters.");
            }

            if (string.IsNullOrWhiteSpace(dto.StaffRole))
            {
                ModelState.AddModelError(nameof(dto.StaffRole), "Staff role is required.");
            }
            else if (dto.StaffRole.Trim().Length > 50)
            {
                ModelState.AddModelError(nameof(dto.StaffRole), "Staff role cannot exceed 50 characters.");
            }

            if (dto.FlatId <= 0)
            {
                ModelState.AddModelError(nameof(dto.FlatId), "Enter a valid Flat ID.");
            }

            if (dto.CheckInTime == default)
            {
                ModelState.AddModelError(nameof(dto.CheckInTime), "Check-in time is required.");
            }

            if (dto.CheckOutTime.HasValue &&
                dto.CheckOutTime.Value < dto.CheckInTime)
            {
                ModelState.AddModelError(
                    nameof(dto.CheckOutTime),
                    "Check-out time cannot be earlier than check-in time.");
            }

            if (dto.TenantId <= 0)
            {
                ModelState.AddModelError(nameof(dto.TenantId), "Enter a valid Tenant ID.");
            }
        }
    }
}
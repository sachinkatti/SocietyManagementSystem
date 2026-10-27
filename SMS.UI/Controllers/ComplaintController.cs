
using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class ComplaintController : Controller
    {
        private readonly IComplaintService _complaintService;
        private readonly ITenantService _tenantService;

        public ComplaintController(
            IComplaintService complaintService,
            ITenantService tenantService)
        {
            _complaintService = complaintService;
            _tenantService = tenantService;
        }

        public async Task<IActionResult> Index()
        {
            var complaints = await _complaintService.GetAllAsync();
            return View(complaints);
        }

        public async Task<IActionResult> Create()
        {
            await LoadTenantsAsync();

            return View(new ComplaintDto
            {
                DateRaised = DateTime.Now,
                Priority = "Medium",
                Status = "Open"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplaintDto complaintDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync();
                return View(complaintDto);
            }

            try
            {
                complaintDto.CreatedBy = GetCurrentUserId();

                await _complaintService.CreateAsync(complaintDto);

                TempData["SuccessMessage"] =
                    "Complaint created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadTenantsAsync();
                return View(complaintDto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var complaint = await _complaintService.GetByIdAsync(id);

            if (complaint == null)
                return NotFound();

            await LoadTenantsAsync();

            return View(complaint);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ComplaintDto complaintDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync();
                return View(complaintDto);
            }

            try
            {
                complaintDto.ModifyBy = GetCurrentUserId();

                bool updated =
                    await _complaintService.UpdateAsync(complaintDto);

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] =
                    "Complaint updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadTenantsAsync();
                return View(complaintDto);
            }
        }

        public async Task<IActionResult> Details(long id)
        {
            var complaint = await _complaintService.GetByIdAsync(id);

            if (complaint == null)
                return NotFound();

            return View(complaint);
        }

        public async Task<IActionResult> Delete(long id)
        {
            var complaint = await _complaintService.GetByIdAsync(id);

            if (complaint == null)
                return NotFound();

            return View(complaint);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            bool deleted = await _complaintService.DeleteAsync(
                id, GetCurrentUserId());

            if (!deleted)
                return NotFound();

            TempData["SuccessMessage"] =
                "Complaint deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetResidentsByTenant(long tenantId)
        {
            if (tenantId <= 0)
                return BadRequest("Invalid tenant ID.");

            var residents =
                await _complaintService.GetResidentsByTenantAsync(tenantId);

            return Json(residents);
        }

        private async Task LoadTenantsAsync()
        {
            // Assumes your existing tenant service returns tenant DTOs
            // with TenantId and TenantName properties.
            var tenants = await _tenantService.GetAllAsync();

            ViewBag.Tenants = tenants;
        }

        private long? GetCurrentUserId()
        {
            // Change "UserId" if your login stores the ID under
            // a different session key.
            var userId = HttpContext.Session.GetString("UserId");

            return long.TryParse(userId, out long parsedUserId)
                ? parsedUserId
                : null;
        }
    }
}

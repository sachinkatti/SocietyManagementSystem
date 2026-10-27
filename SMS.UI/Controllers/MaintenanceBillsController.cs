
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class MaintenanceBillsController : Controller
    {
        private readonly IMaintenanceBillService _maintenanceBillService;
        private readonly ITenantService _tenantService;

        public MaintenanceBillsController(
            IMaintenanceBillService maintenanceBillService,
            ITenantService tenantService)
        {
            _maintenanceBillService = maintenanceBillService;
            _tenantService = tenantService;
        }

        private long? GetUserId()
        {
            var userId = HttpContext.Session.GetString("UserId");

            return long.TryParse(userId, out var id) ? id : null;
        }

        private long? GetTenantId()
        {
            var tenantId = User.FindFirst("TenantId")?.Value;

            return long.TryParse(tenantId, out var id) ? id : null;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            var bills = await _maintenanceBillService.GetByTenantAsync(
                tenantId.Value);

            return View(bills);
        }

        public async Task<IActionResult> Details(long id)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            var dto = await _maintenanceBillService.GetByIdAsync(id);

            if (dto == null || dto.TenantId != tenantId.Value)
                return NotFound();

            return View(dto);
        }

        public async Task<IActionResult> Create()
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            await LoadFlatsAsync(tenantId.Value);

            var dto = new MaintenanceBillDto
            {
                TenantId = tenantId.Value,
                BillingMonth = DateTime.Today.Month,
                BillingYear = DateTime.Today.Year,
                DueDate = DateTime.Today.AddDays(10),
                PaymentStatus = "Pending"
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaintenanceBillDto dto)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            dto.TenantId = tenantId.Value;

            if (!ModelState.IsValid)
            {
                await LoadFlatsAsync(tenantId.Value);
                return View(dto);
            }

            try
            {
                var billId = await _maintenanceBillService.CreateAsync(
                    dto, GetUserId());

                TempData["SuccessMessage"] =
                    $"Maintenance bill created successfully. Bill ID: {billId}";

                return RedirectToAction(nameof(Index));
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadFlatsAsync(tenantId.Value);
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            var dto = await _maintenanceBillService.GetByIdAsync(id);

            if (dto == null || dto.TenantId != tenantId.Value)
                return NotFound();

            await LoadFlatsAsync(tenantId.Value);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            long id, MaintenanceBillDto dto)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            if (id != dto.BillId)
                return BadRequest();

            dto.TenantId = tenantId.Value;

            if (!ModelState.IsValid)
            {
                await LoadFlatsAsync(tenantId.Value);
                return View(dto);
            }

            try
            {
                var updated = await _maintenanceBillService.UpdateAsync(
                    dto, GetUserId());

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] =
                    "Maintenance bill updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadFlatsAsync(tenantId.Value);
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            var deleted = await _maintenanceBillService.DeleteAsync(
                id, GetUserId(), tenantId.Value);

            TempData[deleted ? "SuccessMessage" : "ErrorMessage"] =
                deleted
                    ? "Maintenance bill deleted successfully."
                    : "Maintenance bill not found.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadFlatsAsync(long tenantId)
        {
            var flats = await _maintenanceBillService.GetFlatsByTenantAsync(tenantId);

            ViewBag.Flats = flats.Select(flat => new SelectListItem
            {
                Value = flat.FlatId.ToString(),
                Text = flat.FlatNumber
            }).ToList();
        }

    }
}
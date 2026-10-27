
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SMS.Application;
using System.Security.Claims;

namespace SMS.UI.Controllers
{
    public class ExpenseLedgerController : Controller
    {
        private readonly IExpenseLedgerService _expenseLedgerService;

        public ExpenseLedgerController(
            IExpenseLedgerService expenseLedgerService)
        {
            _expenseLedgerService = expenseLedgerService;
        }

        private long? GetUserId()
        {
            var userId = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            return long.TryParse(userId, out var id) ? id : null;
        }

        private long? GetTenantId()
        {
            var tenantId = User.FindFirst("TenantId")?.Value;

            return long.TryParse(tenantId, out var id) ? id : null;
        }

        // GET: ExpenseLedger
        public async Task<IActionResult> Index()
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            var entries = await _expenseLedgerService
                .GetByTenantAsync(tenantId.Value);

            return View(entries);
        }

        // GET: ExpenseLedger/Details/1
        public async Task<IActionResult> Details(long id)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            var dto = await _expenseLedgerService.GetByIdAsync(id);

            if (dto == null || dto.TenantId != tenantId.Value)
                return NotFound();

            return View(dto);
        }

        // GET: ExpenseLedger/Create
        public async Task<IActionResult> Create()
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            await LoadAdminsAsync(tenantId.Value);

            var dto = new ExpenseLedgerDto
            {
                TenantId = tenantId.Value,
                EntryType = "Expense",
                EntryDate = DateTime.Today
            };

            return View(dto);
        }

        // POST: ExpenseLedger/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseLedgerDto dto)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            dto.TenantId = tenantId.Value;

            if (!ModelState.IsValid)
            {
                await LoadAdminsAsync(tenantId.Value);
                return View(dto);
            }

            try
            {
                var ledgerId = await _expenseLedgerService.CreateAsync(
                    dto, GetUserId());

                TempData["SuccessMessage"] =
                    $"Expense ledger entry created successfully. ID: {ledgerId}";

                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadAdminsAsync(tenantId.Value);
                return View(dto);
            }
        }

        // GET: ExpenseLedger/Edit/1
        public async Task<IActionResult> Edit(long id)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            var dto = await _expenseLedgerService.GetByIdAsync(id);

            if (dto == null || dto.TenantId != tenantId.Value)
                return NotFound();

            await LoadAdminsAsync(tenantId.Value);

            return View(dto);
        }

        // POST: ExpenseLedger/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            long id, ExpenseLedgerDto dto)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            if (id != dto.LedgerId)
                return BadRequest();

            dto.TenantId = tenantId.Value;

            if (!ModelState.IsValid)
            {
                await LoadAdminsAsync(tenantId.Value);
                return View(dto);
            }

            try
            {
                var updated = await _expenseLedgerService.UpdateAsync(
                    dto, GetUserId());

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] =
                    "Expense ledger entry updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadAdminsAsync(tenantId.Value);
                return View(dto);
            }
        }

        // POST: ExpenseLedger/Delete/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
                return RedirectToAction("Login", "Account");

            var deleted = await _expenseLedgerService.DeleteAsync(
                id, GetUserId(), tenantId.Value);

            TempData[deleted ? "SuccessMessage" : "ErrorMessage"] =
                deleted
                    ? "Expense ledger entry deleted successfully."
                    : "Expense ledger entry not found.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadAdminsAsync(long tenantId)
        {
            var admins = await _expenseLedgerService
                .GetAdminsByTenantAsync(tenantId);

            ViewBag.Admins = admins.Select(admin => new SelectListItem
            {
                Value = admin.RecordedByAdminId.ToString(),
                Text = $"{admin.RecordedByAdminName} - {admin.Designation}"
            }).ToList();
        }
    }
}
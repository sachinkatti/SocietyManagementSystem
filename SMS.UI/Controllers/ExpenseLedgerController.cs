using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class ExpenseLedgerController : Controller
    {
        private readonly IExpenseLedgerService _service;

        public ExpenseLedgerController(IExpenseLedgerService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var ledgers = await _service.GetAllAsync();
            return View(ledgers);
        }

        public async Task<IActionResult> Details(long id)
        {
            var ledger = await _service.GetByIdAsync(id);

            if (ledger == null)
            {
                return NotFound();
            }

            return View(ledger);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ExpenseLedgerDto
            {
                EntryDate = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseLedgerDto dto)
        {
            ValidateLedger(dto);

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
            var ledger = await _service.GetByIdAsync(id);

            if (ledger == null)
            {
                return NotFound();
            }

            return View(ledger);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ExpenseLedgerDto dto)
        {
            if (id != dto.LedgerId)
            {
                return BadRequest();
            }

            ValidateLedger(dto);

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
            var ledger = await _service.GetByIdAsync(id);

            if (ledger == null)
            {
                return NotFound();
            }

            return View(ledger);
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

        private void ValidateLedger(ExpenseLedgerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EntryType) || dto.EntryType.Trim().Length > 10)
            {
                ModelState.AddModelError(nameof(dto.EntryType),
                    "Entry Type is required and must not exceed 10 characters.");
            }

            if (string.IsNullOrWhiteSpace(dto.Category) || dto.Category.Trim().Length > 50)
            {
                ModelState.AddModelError(nameof(dto.Category),
                    "Category is required and must not exceed 50 characters.");
            }

            if (dto.Amount <= 0)
            {
                ModelState.AddModelError(nameof(dto.Amount),
                    "Amount must be greater than zero.");
            }

            if (dto.Description?.Length > 250)
            {
                ModelState.AddModelError(nameof(dto.Description),
                    "Description must not exceed 250 characters.");
            }

            if (dto.EntryDate == default)
            {
                ModelState.AddModelError(nameof(dto.EntryDate),
                    "Entry Date is required.");
            }

            if (dto.RecordedByAdminId <= 0)
            {
                ModelState.AddModelError(nameof(dto.RecordedByAdminId),
                    "Enter a valid Admin ID.");
            }

            if (dto.TenantId <= 0)
            {
                ModelState.AddModelError(nameof(dto.TenantId),
                    "Enter a valid Tenant ID.");
            }
        }
    }
}
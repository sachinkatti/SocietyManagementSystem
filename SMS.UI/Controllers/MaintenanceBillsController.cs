using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class MaintenanceBillsController : Controller
    {
        private readonly IMaintenanceBillService _service;

        public MaintenanceBillsController(IMaintenanceBillService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var bills = await _service.GetAllAsync();
            return View(bills);
        }

        public async Task<IActionResult> Details(long id)
        {
            var bill = await _service.GetByIdAsync(id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new MaintenanceBillDto
            {
                BillingMonth = DateTime.Today.Month,
                BillingYear = DateTime.Today.Year,
                DueDate = DateTime.Today.AddDays(10),
                PaymentStatus = "Pending",
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaintenanceBillDto dto)
        {
            ValidateBill(dto);

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
            var bill = await _service.GetByIdAsync(id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, MaintenanceBillDto dto)
        {
            if (id != dto.BillId)
            {
                return BadRequest();
            }

            ValidateBill(dto);

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
            var bill = await _service.GetByIdAsync(id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
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

        private void ValidateBill(MaintenanceBillDto dto)
        {
            if (dto.FlatId <= 0)
            {
                ModelState.AddModelError(nameof(dto.FlatId), "Enter a valid Flat ID.");
            }

            if (dto.BillingMonth < 1 || dto.BillingMonth > 12)
            {
                ModelState.AddModelError(nameof(dto.BillingMonth), "Month must be between 1 and 12.");
            }

            if (dto.BillingYear < 2000 || dto.BillingYear > 9999)
            {
                ModelState.AddModelError(nameof(dto.BillingYear), "Enter a valid billing year.");
            }

            if (dto.BaseAmount < 0)
            {
                ModelState.AddModelError(nameof(dto.BaseAmount), "Base amount cannot be negative.");
            }

            if (dto.PenaltyAmount < 0)
            {
                ModelState.AddModelError(nameof(dto.PenaltyAmount), "Penalty amount cannot be negative.");
            }

            if (dto.BaseAmount + dto.PenaltyAmount > 9999999999999999.99m)
            {
                ModelState.AddModelError(nameof(dto.BaseAmount), "The total amount is too large.");
            }

            if (dto.DueDate == default)
            {
                ModelState.AddModelError(nameof(dto.DueDate), "Due date is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentStatus))
            {
                ModelState.AddModelError(nameof(dto.PaymentStatus), "Payment status is required.");
            }
            else if (dto.PaymentStatus.Trim().Length > 20)
            {
                ModelState.AddModelError(nameof(dto.PaymentStatus), "Payment status cannot exceed 20 characters.");
            }

            if (dto.TransactionRef?.Length > 100)
            {
                ModelState.AddModelError(nameof(dto.TransactionRef), "Transaction reference cannot exceed 100 characters.");
            }

            if (dto.Description?.Length > 500)
            {
                ModelState.AddModelError(nameof(dto.Description), "Description cannot exceed 500 characters.");
            }

            if (dto.TenantId <= 0)
            {
                ModelState.AddModelError(nameof(dto.TenantId), "Enter a valid Tenant ID.");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ITenantService _tenantService;
        private readonly ITenantSubscriptionService _subscriptionService;

        public InvoiceController(
            IInvoiceService invoiceService,
            ITenantService tenantService,
            ITenantSubscriptionService subscriptionService)
        {
            _invoiceService = invoiceService;
            _tenantService = tenantService;
            _subscriptionService = subscriptionService;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return View(invoices);
        }

        public async Task<IActionResult> Details(long id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();

            return View(new InvoiceDto
            {
                InvoiceDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(30),
                Status = "Pending"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceDto invoiceDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(invoiceDto.TenantId);
                return View(invoiceDto);
            }

            try
            {
                await _invoiceService.CreateAsync(invoiceDto);
                TempData["SuccessMessage"] = "Invoice created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create invoice. Check the invoice number and selected IDs.");

                await LoadDropdownsAsync(invoiceDto.TenantId);
                return View(invoiceDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);

            if (invoice == null)
                return NotFound();

            await LoadDropdownsAsync(invoice.TenantId);

            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InvoiceDto invoiceDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(invoiceDto.TenantId);
                return View(invoiceDto);
            }

            try
            {
                bool updated = await _invoiceService.UpdateAsync(invoiceDto);

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] = "Invoice updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update invoice. Check the invoice number and selected IDs.");

                await LoadDropdownsAsync(invoiceDto.TenantId);
                return View(invoiceDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            bool deleted = await _invoiceService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["SuccessMessage"] = "Invoice deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync(long? selectedTenantId = null)
        {
            var tenants = await _tenantService.GetAllAsync();

            ViewBag.Tenants = tenants.Select(t => new SelectListItem
            {
                Value = t.TenantId.ToString(),
                Text = t.TenantName,
                Selected = selectedTenantId.HasValue
                    && t.TenantId == selectedTenantId.Value
            }).ToList();

            var subscriptions = await _subscriptionService.GetAllAsync();

            ViewBag.Subscriptions = subscriptions.Select(s => new SelectListItem
            {
                Value = s.SubscriptionId.ToString(),
                Text = $"Subscription #{s.SubscriptionId} - Tenant #{s.TenantId}",
                Selected = false
            }).ToList();
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IInvoiceService _invoiceService;

        public PaymentController(
            IPaymentService paymentService,
            IInvoiceService invoiceService)
        {
            _paymentService = paymentService;
            _invoiceService = invoiceService;
        }

        private async Task LoadInvoicesAsync()
        {
            var invoices = await _invoiceService.GetAllAsync();

            ViewBag.Invoices = invoices.Select(invoice =>
                new SelectListItem
                {
                    Value = invoice.InvoiceId.ToString(),
                    Text = invoice.InvoiceNumber
                }).ToList();
        }

        public async Task<IActionResult> Index()
        {
            var payments = await _paymentService.GetAllAsync();
            return View(payments);
        }

        public async Task<IActionResult> Details(long id)
        {
            var payment = await _paymentService.GetByIdAsync(id);

            if (payment == null)
                return NotFound();

            return View(payment);
        }

        public async Task<IActionResult> Create()
        {
            await LoadInvoicesAsync();

            return View(new PaymentDto
            {
                PaymentDate = DateTime.Now,
                Status = "Pending"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadInvoicesAsync();
                return View(paymentDto);
            }

            try
            {
                await _paymentService.CreateAsync(paymentDto);
                TempData["Success"] = "Payment created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadInvoicesAsync();
                return View(paymentDto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var payment = await _paymentService.GetByIdAsync(id);

            if (payment == null)
                return NotFound();

            await LoadInvoicesAsync();
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadInvoicesAsync();
                return View(paymentDto);
            }

            try
            {
                bool updated = await _paymentService.UpdateAsync(paymentDto);

                if (!updated)
                    return NotFound();

                TempData["Success"] = "Payment updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadInvoicesAsync();
                return View(paymentDto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var payment = await _paymentService.GetByIdAsync(id);

            if (payment == null)
                return NotFound();

            return View(payment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            bool deleted = await _paymentService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["Success"] = "Payment deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}

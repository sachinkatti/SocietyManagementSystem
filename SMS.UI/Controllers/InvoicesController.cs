using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService _service;

        public InvoicesController(IInvoiceService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _service.GetAllAsync();
            return View(invoices);
        }

        public async Task<IActionResult> Details(long id)
        {
            var invoice = await _service.GetByIdAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new InvoiceDto
            {
                InvoiceDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(30),
                Status = "Pending"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _service.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var invoice = await _service.GetByIdAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, InvoiceDto dto)
        {
            if (id != dto.InvoiceId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                // Fetch original to keep CreatedOn/CreatedBy intact if not sent by form
                var existing = await _service.GetByIdAsync(id);
                if (existing != null)
                {
                    dto.CreatedOn ??= existing.CreatedOn;
                    dto.CreatedBy ??= existing.CreatedBy;
                }

                var updated = await _service.UpdateAsync(dto);

                if (!updated)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id) // Note: standard task signature uses Task<IActionResult>
        {
            var invoice = await _service.GetByIdAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
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
    }
}
using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class TenantSubscriptionsController : Controller
    {
        private readonly ITenantSubscriptionService _service;

        public TenantSubscriptionsController(ITenantSubscriptionService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var subscriptions = await _service.GetAllAsync();
            return View(subscriptions);
        }

        public async Task<IActionResult> Details(long id)
        {
            var subscription = await _service.GetByIdAsync(id);

            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TenantSubscriptionDto
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                Status = "Active"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TenantSubscriptionDto dto)
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
            var subscription = await _service.GetByIdAsync(id);

            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, TenantSubscriptionDto dto)
        {
            if (id != dto.SubscriptionId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
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
        public async Task<IActionResult> Delete(long id)
        {
            var subscription = await _service.GetByIdAsync(id);

            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
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
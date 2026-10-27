
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class TenantSubscriptionController : Controller
    {
        private readonly ITenantSubscriptionService _subscriptionService;
        private readonly ITenantService _tenantService;
        private readonly IPlanService _planService;

        public TenantSubscriptionController(
            ITenantSubscriptionService subscriptionService,
            ITenantService tenantService,
            IPlanService planService)
        {
            _subscriptionService = subscriptionService;
            _tenantService = tenantService;
            _planService = planService;
        }

        public async Task<IActionResult> Index()
        {
            var subscriptions = await _subscriptionService.GetAllAsync();
            return View(subscriptions);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();

            return View(new TenantSubscriptionDto
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                Status = "Active",
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TenantSubscriptionDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(dto.TenantId, dto.PlanId);
                return View(dto);
            }

            try
            {
                await _subscriptionService.CreateAsync(dto);

                TempData["SuccessMessage"] =
                    "Subscription created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            await LoadDropdownsAsync(dto.TenantId, dto.PlanId);
            return View(dto);
        }

        public async Task<IActionResult> Edit(long id)
        {
            var subscription =
                await _subscriptionService.GetByIdAsync(id);

            if (subscription == null)
                return NotFound();

            await LoadDropdownsAsync(
                subscription.TenantId,
                subscription.PlanId);

            return View(subscription);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TenantSubscriptionDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(dto.TenantId, dto.PlanId);
                return View(dto);
            }

            try
            {
                var updated =
                    await _subscriptionService.UpdateAsync(dto);

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] =
                    "Subscription updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            await LoadDropdownsAsync(dto.TenantId, dto.PlanId);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted =
                await _subscriptionService.DeleteAsync(id);

            TempData[deleted ? "SuccessMessage" : "ErrorMessage"] =
                deleted
                    ? "Subscription deleted successfully."
                    : "Subscription was not found.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync(
            long? selectedTenantId = null,
            long? selectedPlanId = null)
        {
            var tenants = await _tenantService.GetAllAsync();

            ViewBag.Tenants = new SelectList(
                tenants,
                "TenantId",
                "TenantName",
                selectedTenantId);

            var plans = await _planService.GetAllAsync();

            ViewBag.Plans = new SelectList(
                plans,
                "PlanId",
                "PlanName",
                selectedPlanId);
        }
    }
}
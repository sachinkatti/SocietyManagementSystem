
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class PlanFeaturesController : Controller
    {
        private readonly IPlanFeatureService _planFeatureService;
        private readonly IPlanService _planService;
        private readonly IFeatureService _featureService;

        public PlanFeaturesController(
            IPlanFeatureService planFeatureService,
            IPlanService planService,
            IFeatureService featureService)
        {
            _planFeatureService = planFeatureService;
            _planService = planService;
            _featureService = featureService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _planFeatureService.GetAllAsync();
            return View(items);
        }

        public async Task<IActionResult> Details(long id)
        {
            var item = await _planFeatureService.GetByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View(new PlanFeatureDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanFeatureDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(dto);
            }

            try
            {
                await _planFeatureService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Plan feature mapping created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create this mapping. The plan-feature combination may already exist, or the selected plan or feature may be inactive."
                );

                await LoadDropdownsAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var item = await _planFeatureService.GetByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await LoadDropdownsAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, PlanFeatureDto dto)
        {
            if (id != dto.PlanFeatureId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(dto);
            }

            try
            {
                var updated = await _planFeatureService.UpdateAsync(dto);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Plan feature mapping updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update this mapping. Check the selected plan and feature."
                );

                await LoadDropdownsAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var item = await _planFeatureService.GetByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _planFeatureService.DeleteAsync(id, null);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Plan feature mapping deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync()
        {
            var plans = await _planService.GetAllAsync();
            var features = await _featureService.GetAllAsync();

            ViewBag.Plans = new SelectList(plans, "PlanId", "PlanName");
            ViewBag.Features = new SelectList(features, "FeatureId", "FeatureName");
        }
    }
}

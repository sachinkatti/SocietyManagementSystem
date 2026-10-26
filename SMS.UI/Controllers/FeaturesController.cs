
using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class FeaturesController : Controller
    {
        private readonly IFeatureService _featureService;

        public FeaturesController(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        public async Task<IActionResult> Index()
        {
            var features = await _featureService.GetAllAsync();
            return View(features);
        }

        public async Task<IActionResult> Details(long id)
        {
            var feature = await _featureService.GetByIdAsync(id);

            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        public IActionResult Create()
        {
            return View(new FeatureDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeatureDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _featureService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Feature created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the feature. Check whether the feature code already exists."
                );

                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var feature = await _featureService.GetByIdAsync(id);

            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, FeatureDto dto)
        {
            if (id != dto.FeatureId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var updated = await _featureService.UpdateAsync(dto);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Feature updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the feature. Check whether the feature code already exists."
                );

                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var feature = await _featureService.GetByIdAsync(id);

            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _featureService.DeleteAsync(id, null);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Feature deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
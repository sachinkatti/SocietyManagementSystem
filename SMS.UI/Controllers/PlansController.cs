
using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        public async Task<IActionResult> Index()
        {
            var plans = await _planService.GetAllAsync();
            return View(plans);
        }

        public async Task<IActionResult> Details(long id)
        {
            var plan = await _planService.GetByIdAsync(id);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        public IActionResult Create()
        {
            return View(new PlanDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _planService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Plan created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the plan. Check whether the plan code already exists."
                );

                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var plan = await _planService.GetByIdAsync(id);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, PlanDto dto)
        {
            if (id != dto.PlanId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var updated = await _planService.UpdateAsync(dto);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Plan updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the plan. Check whether the plan code already exists."
                );

                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var plan = await _planService.GetByIdAsync(id);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _planService.DeleteAsync(id, null);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Plan deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}

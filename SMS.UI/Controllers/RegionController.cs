using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class RegionController : Controller
    {
        private readonly IRegionService _regionService;

        public RegionController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        public async Task<IActionResult> Index()
        {
            var regions = await _regionService.GetAllAsync();

            return View(regions);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _regionService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var region = await _regionService.GetByIdAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            return View(region);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var region = await _regionService.GetByIdAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            return View(region);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _regionService.UpdateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var region = await _regionService.GetByIdAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            return View(region);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long regionId)
        {
            await _regionService.DeleteAsync(regionId);

            return RedirectToAction(nameof(Index));
        }
    }
}
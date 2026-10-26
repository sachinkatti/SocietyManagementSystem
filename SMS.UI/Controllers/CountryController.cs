using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountryService _countryService;
        private readonly IRegionService _regionService;

        public CountryController(
            ICountryService countryService,
            IRegionService regionService)
        {
            _countryService = countryService;
            _regionService = regionService;
        }

        public async Task<IActionResult> Index()
        {
            var countries = await _countryService.GetAllAsync();

            var regions = await _regionService.GetAllAsync();

            var regionLookup = regions.ToDictionary(
                x => x.RegionId,
                x => x.RegionName);

            foreach (var country in countries)
            {
                if (regionLookup.TryGetValue(country.RegionId, out var regionName))
                {
                    country.RegionName = regionName;
                }
            }

            return View(countries);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadRegions();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadRegions(dto.RegionId);
                return View(dto);
            }

            await _countryService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var country = await _countryService.GetByIdAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            var regions = await _regionService.GetAllAsync();

            var region = regions.FirstOrDefault(x => x.RegionId == country.RegionId);

            country.RegionName = region?.RegionName;

            return View(country);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var country = await _countryService.GetByIdAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            await LoadRegions(country.RegionId);

            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CountryDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadRegions(dto.RegionId);
                return View(dto);
            }

            var result = await _countryService.UpdateAsync(dto);

            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var country = await _countryService.GetByIdAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            var regions = await _regionService.GetAllAsync();

            var region = regions.FirstOrDefault(x => x.RegionId == country.RegionId);

            country.RegionName = region?.RegionName;

            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long countryId)
        {
            await _countryService.DeleteAsync(countryId);

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadRegions(long? selectedRegionId = null)
        {
            var regions = await _regionService.GetAllAsync();

            ViewBag.Regions = new SelectList(
                regions,
                "RegionId",
                "RegionName",
                selectedRegionId);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class CityController : Controller
    {
        private readonly ICityService _cityService;
        private readonly ITalukService _talukService;

        public CityController(
            ICityService cityService,
            ITalukService talukService)
        {
            _cityService = cityService;
            _talukService = talukService;
        }

        public async Task<IActionResult> Index()
        {
            var cities = await _cityService.GetAllAsync();
            var taluks = await _talukService.GetAllAsync();

            var talukDictionary = taluks
                .ToDictionary(x => x.TalukId, x => x.TalukName);

            foreach (var city in cities)
            {
                if (talukDictionary.TryGetValue(
                    city.TalukId,
                    out var talukName))
                {
                    city.TalukName = talukName;
                }
            }

            return View(cities);
        }

        public async Task<IActionResult> Details(long id)
        {
            var city = await _cityService.GetByIdAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            var taluk = await _talukService.GetByIdAsync(
                city.TalukId);

            city.TalukName = taluk?.TalukName;

            return View(city);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadTaluks();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CityDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadTaluks(dto.TalukId);
                return View(dto);
            }

            await _cityService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var city = await _cityService.GetByIdAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            await LoadTaluks(city.TalukId);

            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CityDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadTaluks(dto.TalukId);
                return View(dto);
            }

            var result = await _cityService.UpdateAsync(dto);

            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var city = await _cityService.GetByIdAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            var taluk = await _talukService.GetByIdAsync(
                city.TalukId);

            city.TalukName = taluk?.TalukName;

            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long cityId)
        {
            await _cityService.DeleteAsync(cityId);

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadTaluks(
            long? selectedTalukId = null)
        {
            var taluks = await _talukService.GetAllAsync();

            ViewBag.Taluks = new SelectList(
                taluks,
                "TalukId",
                "TalukName",
                selectedTalukId);
        }
    }
}
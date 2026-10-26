using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class TalukController : Controller
    {
        private readonly ITalukService _talukService;
        private readonly IDistrictService _districtService;

        public TalukController(
            ITalukService talukService,
            IDistrictService districtService)
        {
            _talukService = talukService;
            _districtService = districtService;
        }

        public async Task<IActionResult> Index()
        {
            var taluks = await _talukService.GetAllAsync();
            var districts = await _districtService.GetAllAsync();

            var districtDictionary = districts
                .ToDictionary(x => x.DistrictId, x => x.DistrictName);

            foreach (var taluk in taluks)
            {
                if (districtDictionary.TryGetValue(
                    taluk.DistrictId,
                    out var districtName))
                {
                    taluk.DistrictName = districtName;
                }
            }

            return View(taluks);
        }

        public async Task<IActionResult> Details(long id)
        {
            var taluk = await _talukService.GetByIdAsync(id);

            if (taluk == null)
            {
                return NotFound();
            }

            var district = await _districtService.GetByIdAsync(
                taluk.DistrictId);

            taluk.DistrictName = district?.DistrictName;

            return View(taluk);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDistricts();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TalukDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDistricts(dto.DistrictId);
                return View(dto);
            }

            await _talukService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var taluk = await _talukService.GetByIdAsync(id);

            if (taluk == null)
            {
                return NotFound();
            }

            await LoadDistricts(taluk.DistrictId);

            return View(taluk);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TalukDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDistricts(dto.DistrictId);
                return View(dto);
            }

            var result = await _talukService.UpdateAsync(dto);

            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var taluk = await _talukService.GetByIdAsync(id);

            if (taluk == null)
            {
                return NotFound();
            }

            var district = await _districtService.GetByIdAsync(
                taluk.DistrictId);

            taluk.DistrictName = district?.DistrictName;

            return View(taluk);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long talukId)
        {
            await _talukService.DeleteAsync(talukId);

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDistricts(
            long? selectedDistrictId = null)
        {
            var districts = await _districtService.GetAllAsync();

            ViewBag.Districts = new SelectList(
                districts,
                "DistrictId",
                "DistrictName",
                selectedDistrictId);
        }
    }
}
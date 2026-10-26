using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class DistrictController : Controller
    {
        private readonly IDistrictService _districtService;
        private readonly IStateService _stateService;

        public DistrictController(
            IDistrictService districtService,
            IStateService stateService)
        {
            _districtService = districtService;
            _stateService = stateService;
        }

        public async Task<IActionResult> Index()
        {
            var districts = await _districtService.GetAllAsync();
            var states = await _stateService.GetAllAsync();

            var stateDictionary = states
                .ToDictionary(x => x.StateId, x => x.StateName);

            foreach (var district in districts)
            {
                if (stateDictionary.TryGetValue(
                    district.StateId,
                    out var stateName))
                {
                    district.StateName = stateName;
                }
            }

            return View(districts);
        }

        public async Task<IActionResult> Details(long id)
        {
            var district = await _districtService.GetByIdAsync(id);

            if (district == null)
            {
                return NotFound();
            }

            var state = await _stateService.GetByIdAsync(
                district.StateId);

            district.StateName = state?.StateName;

            return View(district);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadStates();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DistrictDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadStates(dto.StateId);
                return View(dto);
            }

            await _districtService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var district = await _districtService.GetByIdAsync(id);

            if (district == null)
            {
                return NotFound();
            }

            await LoadStates(district.StateId);

            return View(district);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DistrictDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadStates(dto.StateId);
                return View(dto);
            }

            var result = await _districtService.UpdateAsync(dto);

            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var district = await _districtService.GetByIdAsync(id);

            if (district == null)
            {
                return NotFound();
            }

            var state = await _stateService.GetByIdAsync(
                district.StateId);

            district.StateName = state?.StateName;

            return View(district);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long districtId)
        {
            await _districtService.DeleteAsync(districtId);

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadStates(long? selectedStateId = null)
        {
            var states = await _stateService.GetAllAsync();

            ViewBag.States = new SelectList(
                states,
                "StateId",
                "StateName",
                selectedStateId);
        }
    }
}
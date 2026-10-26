using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class StateController : Controller
    {
        private readonly IStateService _stateService;
        private readonly ICountryService _countryService;

        public StateController(
            IStateService stateService,
            ICountryService countryService)
        {
            _stateService = stateService;
            _countryService = countryService;
        }

        public async Task<IActionResult> Index()
        {
            var states = await _stateService.GetAllAsync();
            var countries = await _countryService.GetAllAsync();

            var countryDictionary = countries
                .ToDictionary(x => x.CountryId, x => x.CountryName);

            foreach (var state in states)
            {
                if (countryDictionary.TryGetValue(
                    state.CountryId,
                    out var countryName))
                {
                    state.CountryName = countryName;
                }
            }

            return View(states);
        }

        public async Task<IActionResult> Details(long id)
        {
            var state = await _stateService.GetByIdAsync(id);

            if (state == null)
            {
                return NotFound();
            }

            var country = await _countryService.GetByIdAsync(
                state.CountryId);

            state.CountryName = country?.CountryName;

            return View(state);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCountries();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCountries(dto.CountryId);
                return View(dto);
            }

            await _stateService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var state = await _stateService.GetByIdAsync(id);

            if (state == null)
            {
                return NotFound();
            }

            await LoadCountries(state.CountryId);

            return View(state);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCountries(dto.CountryId);
                return View(dto);
            }

            var result = await _stateService.UpdateAsync(dto);

            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var state = await _stateService.GetByIdAsync(id);

            if (state == null)
            {
                return NotFound();
            }

            var country = await _countryService.GetByIdAsync(
                state.CountryId);

            state.CountryName = country?.CountryName;

            return View(state);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long stateId)
        {
            await _stateService.DeleteAsync(stateId);

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCountries(long? selectedCountryId = null)
        {
            var countries = await _countryService.GetAllAsync();

            ViewBag.Countries = new SelectList(
                countries,
                "CountryId",
                "CountryName",
                selectedCountryId);
        }
    }
}
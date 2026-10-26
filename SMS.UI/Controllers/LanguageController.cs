using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class LanguageController : Controller
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        public async Task<IActionResult> Index()
        {
            var languages = await _languageService.GetAllAsync();

            return View(languages);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LanguageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _languageService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var language = await _languageService.GetByIdAsync(id);

            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var language = await _languageService.GetByIdAsync(id);

            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LanguageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _languageService.UpdateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var language = await _languageService.GetByIdAsync(id);

            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long languageId)
        {
            await _languageService.DeleteAsync(languageId);

            return RedirectToAction(nameof(Index));
        }
    }
}
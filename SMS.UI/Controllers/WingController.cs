
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class WingController : Controller
    {
        private readonly IWingService _wingService;
        private readonly ITenantService _tenantService;

        public WingController(
            IWingService wingService,
            ITenantService tenantService)
        {
            _wingService = wingService;
            _tenantService = tenantService;
        }

        // GET: Wing
        public async Task<IActionResult> Index()
        {
            var wings = await _wingService.GetAllAsync();
            return View(wings);
        }

        // GET: Wing/Details/1
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var wing = await _wingService.GetByIdAsync(id.Value);

            if (wing == null)
            {
                return NotFound();
            }

            return View(wing);
        }

        // GET: Wing/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadTenantDropdown();
            return View();
        }

        // POST: Wing/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WingDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadTenantDropdown(dto.TenantId);
                return View(dto);
            }

            await _wingService.CreateAsync(dto);

            TempData["SuccessMessage"] = "Wing created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Wing/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var wing = await _wingService.GetByIdAsync(id.Value);

            if (wing == null)
            {
                return NotFound();
            }

            await LoadTenantDropdown(wing.TenantId);
            return View(wing);
        }

        // POST: Wing/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, WingDto dto)
        {
            if (id != dto.WingId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadTenantDropdown(dto.TenantId);
                return View(dto);
            }

            try
            {
                var updated = await _wingService.UpdateAsync(dto);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Wing updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the wing. Check the tenant and ensure the wing name is not duplicated for that tenant.");

                LoadTenantDropdown(dto.TenantId);
                return View(dto);
            }
        }

        // GET: Wing/Delete/1
        [HttpGet]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var wing = await _wingService.GetByIdAsync(id.Value);

            if (wing == null)
            {
                return NotFound();
            }

            return View(wing);
        }

        // POST: Wing/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _wingService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Wing deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadTenantDropdown(long? selectedTenantId = null)
        {
            var tenants = await _tenantService.GetAllAsync();

            ViewBag.Tenants = tenants.Select(t => new SelectListItem
            {
                Value = t.TenantId.ToString(),
                Text = t.TenantName,
                Selected = selectedTenantId.HasValue &&
                           selectedTenantId.Value == t.TenantId
            }).ToList();
        }
    }
}

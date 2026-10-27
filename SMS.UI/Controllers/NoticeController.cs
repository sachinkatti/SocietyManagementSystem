using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class NoticeController : Controller
    {
        private readonly INoticeService _noticeService;
        private readonly ITenantService _tenantService;

        public NoticeController(
            INoticeService noticeService,
            ITenantService tenantService)
        {
            _noticeService = noticeService;
            _tenantService = tenantService;
        }

        // GET: Notice
        public async Task<IActionResult> Index()
        {
            var notices = await _noticeService.GetAllAsync();
            return View(notices);
        }

        // GET: Notice/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var notice = await _noticeService.GetByIdAsync(id);

            if (notice == null)
            {
                return NotFound();
            }

            return View(notice);
        }

        // GET: Notice/Create
        public async Task<IActionResult> Create()
        {
            var model = new NoticeDto();

            await LoadDropdownsAsync(model);

            return View(model);
        }

        // POST: Notice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoticeDto model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(model);
                return View(model);
            }

            try
            {
                model.CreatedBy = GetCurrentUserId();

                await _noticeService.CreateAsync(model);

                TempData["SuccessMessage"] = "Notice created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            await LoadDropdownsAsync(model);
            return View(model);
        }

        // GET: Notice/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var notice = await _noticeService.GetByIdAsync(id);

            if (notice == null)
            {
                return NotFound();
            }

            await LoadDropdownsAsync(notice);

            return View(notice);
        }

        // POST: Notice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, NoticeDto model)
        {
            if (id != model.NoticeId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(model);
                return View(model);
            }

            try
            {
                model.ModifyBy = GetCurrentUserId();

                bool updated = await _noticeService.UpdateAsync(model);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Notice updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            await LoadDropdownsAsync(model);
            return View(model);
        }

        // GET: Notice/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var notice = await _noticeService.GetByIdAsync(id);

            if (notice == null)
            {
                return NotFound();
            }

            return View(notice);
        }

        // POST: Notice/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var notice = await _noticeService.GetByIdAsync(id);

            if (notice == null)
            {
                return NotFound();
            }

            try
            {
                long? userId = GetCurrentUserId();

                bool deleted = await _noticeService.DeleteAsync(
                    id,
                    userId,
                    notice.TenantId);

                if (!deleted)
                {
                    TempData["ErrorMessage"] =
                        "Notice could not be deleted.";

                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Notice deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Load tenant and admin dropdowns
        private async Task LoadDropdownsAsync(NoticeDto model)
        {
            var tenants = await _tenantService.GetAllAsync();

            ViewBag.Tenants = new SelectList(
                tenants,
                "TenantId",
                "TenantName",
                model.TenantId);

            var admins = model.TenantId > 0
                ? await _noticeService.GetAdminsByTenantAsync(model.TenantId)
                : new List<NoticeAdminDto>();

            model.Admins = admins;

            ViewBag.Admins = new SelectList(
                admins,
                "AdminId",
                "AdminName",
                model.PublishedByAdminId);
        }

        // Read the logged-in user's ID from session
        private long? GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetString("UserId");

            return long.TryParse(userId, out long parsedUserId)
                ? parsedUserId
                : null;
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class NoticesController : Controller
    {
        private readonly INoticeService _noticeService;

        public NoticesController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        public async Task<IActionResult> Index()
        {
            var notices = await _noticeService.GetAllAsync();
            return View(notices);
        }

        public async Task<IActionResult> Details(long id)
        {
            var notice = await _noticeService.GetByIdAsync(id);

            if (notice == null)
            {
                return NotFound();
            }

            return View(notice);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new NoticeDto
            {
                PublishDate = DateTime.Today,
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoticeDto dto)
        {
            ValidateNotice(dto);

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _noticeService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var notice = await _noticeService.GetByIdAsync(id);

            if (notice == null)
            {
                return NotFound();
            }

            return View(notice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, NoticeDto dto)
        {
            if (id != dto.NoticeId)
            {
                return BadRequest();
            }

            ValidateNotice(dto);

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var updated = await _noticeService.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var notice = await _noticeService.GetByIdAsync(id);

            if (notice == null)
            {
                return NotFound();
            }

            return View(notice);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _noticeService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private void ValidateNotice(NoticeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                ModelState.AddModelError(nameof(dto.Title), "Title is required.");
            }
            else if (dto.Title.Trim().Length > 150)
            {
                ModelState.AddModelError(nameof(dto.Title), "Title cannot exceed 150 characters.");
            }

            if (string.IsNullOrWhiteSpace(dto.Message))
            {
                ModelState.AddModelError(nameof(dto.Message), "Message is required.");
            }
            else if (dto.Message.Trim().Length > 1000)
            {
                ModelState.AddModelError(nameof(dto.Message), "Message cannot exceed 1000 characters.");
            }

            if (string.IsNullOrWhiteSpace(dto.Category))
            {
                ModelState.AddModelError(nameof(dto.Category), "Category is required.");
            }
            else if (dto.Category.Trim().Length > 30)
            {
                ModelState.AddModelError(nameof(dto.Category), "Category cannot exceed 30 characters.");
            }

            if (dto.PublishDate == default)
            {
                ModelState.AddModelError(nameof(dto.PublishDate), "Publish date is required.");
            }

            if (dto.PublishedByAdminId <= 0)
            {
                ModelState.AddModelError(nameof(dto.PublishedByAdminId), "Enter a valid Admin ID.");
            }

            if (dto.TenantId <= 0)
            {
                ModelState.AddModelError(nameof(dto.TenantId), "Enter a valid Tenant ID.");
            }
        }
    }
}
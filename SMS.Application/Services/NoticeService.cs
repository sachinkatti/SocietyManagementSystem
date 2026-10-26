using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class NoticeService : INoticeService
    {
        private readonly INoticeRepository _noticeRepository;

        public NoticeService(INoticeRepository noticeRepository)
        {
            _noticeRepository = noticeRepository;
        }

        public async Task<List<NoticeDto>> GetAllAsync()
        {
            var notices = await _noticeRepository.GetAllAsync();

            return notices.Select(MapToDto).ToList();
        }

        public async Task<NoticeDto?> GetByIdAsync(long id)
        {
            var notice = await _noticeRepository.GetByIdAsync(id);

            return notice == null ? null : MapToDto(notice);
        }

        public async Task<long> CreateAsync(NoticeDto dto)
        {
            var notice = new Notice
            {
                Title = dto.Title.Trim(),
                Message = dto.Message.Trim(),
                Category = dto.Category.Trim(),
                PublishDate = dto.PublishDate,
                PublishedByAdminId = dto.PublishedByAdminId,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy,
                TenantId = dto.TenantId
            };

            return await _noticeRepository.CreateAsync(notice);
        }

        public async Task<bool> UpdateAsync(NoticeDto dto)
        {
            var notice = new Notice
            {
                NoticeId = dto.NoticeId,
                Title = dto.Title.Trim(),
                Message = dto.Message.Trim(),
                Category = dto.Category.Trim(),
                PublishDate = dto.PublishDate,
                PublishedByAdminId = dto.PublishedByAdminId,
                IsActive = true,
                ModifyOn = DateTime.UtcNow,
                ModifyBy = dto.ModifyBy,
                TenantId = dto.TenantId
            };

            return await _noticeRepository.UpdateAsync(notice);
        }

        public Task<bool> DeleteAsync(long id)
        {
            return _noticeRepository.DeleteAsync(id);
        }

        private static NoticeDto MapToDto(Notice notice)
        {
            return new NoticeDto
            {
                NoticeId = notice.NoticeId,
                Title = notice.Title,
                Message = notice.Message,
                Category = notice.Category,
                PublishDate = notice.PublishDate,
                PublishedByAdminId = notice.PublishedByAdminId,
                IsActive = notice.IsActive,
                CreatedOn = notice.CreatedOn,
                CreatedBy = notice.CreatedBy,
                ModifyOn = notice.ModifyOn,
                ModifyBy = notice.ModifyBy,
                TenantId = notice.TenantId
            };
        }
    }
}
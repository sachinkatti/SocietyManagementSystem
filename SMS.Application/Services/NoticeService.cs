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

        // 1. Get all active notices
        public async Task<List<NoticeDto>> GetAllAsync()
        {
            var notices = await _noticeRepository.GetAllAsync();

            return notices.Select(MapToDto).ToList();
        }

        // 2. Get notice by ID
        public async Task<NoticeDto?> GetByIdAsync(long noticeId)
        {
            var notice = await _noticeRepository.GetByIdAsync(noticeId);

            return notice == null ? null : MapToDto(notice);
        }

        // 3. Get notices by tenant
        public async Task<List<NoticeDto>> GetByTenantAsync(long tenantId)
        {
            if (tenantId <= 0)
            {
                throw new ArgumentException("Invalid tenant ID.");
            }

            var notices =
                await _noticeRepository.GetByTenantAsync(tenantId);

            return notices.Select(MapToDto).ToList();
        }

        // 4. Create notice
        public async Task<long> CreateAsync(NoticeDto notice)
        {
            var errors = NoticeValidator.Validate(notice);

            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join(" ", errors));
            }

            var entity = MapToEntity(notice);

            return await _noticeRepository.CreateAsync(entity);
        }

        // 5. Update notice
        public async Task<bool> UpdateAsync(NoticeDto notice)
        {
            if (notice.NoticeId <= 0)
            {
                throw new ArgumentException("Invalid notice ID.");
            }

            var errors = NoticeValidator.Validate(notice);

            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join(" ", errors));
            }

            var entity = MapToEntity(notice);

            return await _noticeRepository.UpdateAsync(entity);
        }

        // 6. Soft-delete notice
        public async Task<bool> DeleteAsync(
            long noticeId,
            long? modifyBy,
            long tenantId)
        {
            if (noticeId <= 0)
            {
                throw new ArgumentException("Invalid notice ID.");
            }

            if (tenantId <= 0)
            {
                throw new ArgumentException("Invalid tenant ID.");
            }

            return await _noticeRepository.DeleteAsync(
                noticeId,
                modifyBy,
                tenantId);
        }

        // 7. Get admins for the dropdown
        public async Task<List<NoticeAdminDto>> GetAdminsByTenantAsync(
            long tenantId)
        {
            if (tenantId <= 0)
            {
                throw new ArgumentException("Invalid tenant ID.");
            }

            var admins =
                await _noticeRepository.GetAdminsByTenantAsync(tenantId);

            return admins.Select(admin => new NoticeAdminDto
            {
                AdminId = admin.AdminId,
                AdminName = admin.AdminName
            }).ToList();
        }

        // Convert entity to DTO
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
                AdminName = notice.AdminName,
                IsActive = notice.IsActive,
                CreatedOn = notice.CreatedOn,
                CreatedBy = notice.CreatedBy,
                ModifyOn = notice.ModifyOn,
                ModifyBy = notice.ModifyBy,
                TenantId = notice.TenantId,
                TenantName = notice.TenantName
            };
        }

        // Convert DTO to entity
        private static Notice MapToEntity(NoticeDto notice)
        {
            return new Notice
            {
                NoticeId = notice.NoticeId,
                Title = notice.Title.Trim(),
                Message = notice.Message.Trim(),
                Category = notice.Category.Trim(),
                PublishDate = notice.PublishDate,
                PublishedByAdminId = notice.PublishedByAdminId,
                IsActive = notice.IsActive,
                CreatedBy = notice.CreatedBy,
                ModifyBy = notice.ModifyBy,
                TenantId = notice.TenantId
            };
        }
    }
}
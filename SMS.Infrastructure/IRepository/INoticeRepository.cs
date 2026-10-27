using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface INoticeRepository
    {
        Task<List<Notice>> GetAllAsync();

        Task<Notice?> GetByIdAsync(long noticeId);

        Task<List<Notice>> GetByTenantAsync(long tenantId);

        Task<long> CreateAsync(Notice notice);

        Task<bool> UpdateAsync(Notice notice);

        Task<bool> DeleteAsync(long noticeId, long? modifyBy, long tenantId);

        Task<List<NoticeAdminOption>> GetAdminsByTenantAsync(long tenantId);
    }

    public class NoticeAdminOption
    {
        public long AdminId { get; set; }

        public string AdminName { get; set; } = string.Empty;
    }
}
namespace SMS.Application
{
    public interface INoticeService
    {
        Task<List<NoticeDto>> GetAllAsync();

        Task<NoticeDto?> GetByIdAsync(long noticeId);

        Task<List<NoticeDto>> GetByTenantAsync(long tenantId);

        Task<long> CreateAsync(NoticeDto notice);

        Task<bool> UpdateAsync(NoticeDto notice);

        Task<bool> DeleteAsync(
            long noticeId,
            long? modifyBy,
            long tenantId);

        Task<List<NoticeAdminDto>> GetAdminsByTenantAsync(
            long tenantId);
    }
}
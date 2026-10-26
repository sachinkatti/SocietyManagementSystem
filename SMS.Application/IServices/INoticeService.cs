namespace SMS.Application
{
    public interface INoticeService
    {
        Task<List<NoticeDto>> GetAllAsync();
        Task<NoticeDto?> GetByIdAsync(long id);
        Task<long> CreateAsync(NoticeDto dto);
        Task<bool> UpdateAsync(NoticeDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface INoticeRepository
    {
        Task<List<Notice>> GetAllAsync();
        Task<Notice?> GetByIdAsync(long id);
        Task<long> CreateAsync(Notice notice);
        Task<bool> UpdateAsync(Notice notice);
        Task<bool> DeleteAsync(long id);
    }
}
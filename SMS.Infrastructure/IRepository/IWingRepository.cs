using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IWingRepository
    {
        Task<IEnumerable<Wing>> GetAllAsync();
        Task<Wing?> GetByIdAsync(long wingId);
        Task<long> CreateAsync(Wing wing);
        Task<bool> UpdateAsync(Wing wing);
        Task<bool> DeleteAsync(long wingId);
    }
}
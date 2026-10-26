using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface ITalukRepository
    {
        Task<IEnumerable<Taluk>> GetAllAsync();
        Task<Taluk?> GetByIdAsync(long talukId);
        Task<long> CreateAsync(Taluk taluk);
        Task<bool> UpdateAsync(Taluk taluk);
        Task<bool> DeleteAsync(long talukId);
    }
}
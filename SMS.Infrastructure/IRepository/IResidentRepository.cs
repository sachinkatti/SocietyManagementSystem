using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IResidentRepository
    {
        Task<IEnumerable<Resident>> GetAllAsync();
        Task<Resident?> GetByIdAsync(long residentId);
        Task<long> CreateAsync(Resident resident);
        Task<bool> UpdateAsync(Resident resident);
        Task<bool> DeleteAsync(long residentId);
    }
}

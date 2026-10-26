
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IPlanRepository
    {
        Task<IEnumerable<Plan>> GetAllAsync();
        Task<Plan?> GetByIdAsync(long planId);
        Task<long> CreateAsync(Plan plan);
        Task<bool> UpdateAsync(Plan plan);
        Task<bool> DeleteAsync(long planId, long? modifyBy);
    }
}

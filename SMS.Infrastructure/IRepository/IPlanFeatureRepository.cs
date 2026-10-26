
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IPlanFeatureRepository
    {
        Task<IEnumerable<PlanFeature>> GetAllAsync();
        Task<PlanFeature?> GetByIdAsync(long planFeatureId);
        Task<long> CreateAsync(PlanFeature planFeature);
        Task<bool> UpdateAsync(PlanFeature planFeature);
        Task<bool> DeleteAsync(long planFeatureId, long? modifyBy);
    }
}

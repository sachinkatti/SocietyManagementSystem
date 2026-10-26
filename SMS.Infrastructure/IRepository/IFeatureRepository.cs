using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IFeatureRepository
    {
        Task<IEnumerable<Feature>> GetAllAsync();
        Task<Feature?> GetByIdAsync(long featureId);
        Task<long> CreateAsync(Feature feature);
        Task<bool> UpdateAsync(Feature feature);
        Task<bool> DeleteAsync(long featureId, long? modifyBy);
    }
}
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IRegionRepository
    {
        Task<IEnumerable<Region>> GetAllAsync();
        Task<Region?> GetByIdAsync(long regionId);
        Task<long> CreateAsync(Region region);
        Task<bool> UpdateAsync(Region region);
        Task<bool> DeleteAsync(long regionId);
    }
}
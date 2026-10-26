
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IAmenityRepository
    {
        Task<IEnumerable<Amenity>> GetAllAsync();
        Task<Amenity?> GetByIdAsync(long amenityId);
        Task<long> CreateAsync(Amenity amenity);
        Task<bool> UpdateAsync(Amenity amenity);
        Task<bool> DeleteAsync(long amenityId);
    }
}

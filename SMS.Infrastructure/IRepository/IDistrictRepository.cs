using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IDistrictRepository
    {
        Task<IEnumerable<District>> GetAllAsync();
        Task<District?> GetByIdAsync(long districtId);
        Task<long> CreateAsync(District district);
        Task<bool> UpdateAsync(District district);
        Task<bool> DeleteAsync(long districtId);
    }
}
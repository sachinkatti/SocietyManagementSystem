using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<City?> GetByIdAsync(long cityId);
        Task<long> CreateAsync(City city);
        Task<bool> UpdateAsync(City city);
        Task<bool> DeleteAsync(long cityId);
    }
}
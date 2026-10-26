using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetAllAsync();
        Task<Country?> GetByIdAsync(long countryId);
        Task<long> CreateAsync(Country country);
        Task<bool> UpdateAsync(Country country);
        Task<bool> DeleteAsync(long countryId);
    }
}
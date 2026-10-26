namespace SMS.Application
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryDto>> GetAllAsync();
        Task<CountryDto?> GetByIdAsync(long countryId);
        Task<long> CreateAsync(CountryDto dto);
        Task<bool> UpdateAsync(CountryDto dto);
        Task<bool> DeleteAsync(long countryId);
    }
}
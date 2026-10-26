using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<IEnumerable<CountryDto>> GetAllAsync()
        {
            var countries = await _countryRepository.GetAllAsync();

            return countries.Select(country => new CountryDto
            {
                CountryId = country.CountryId,
                RegionId = country.RegionId,
                CountryName = country.CountryName,
                CountryCode = country.CountryCode,
                Description = country.Description,
                IsActive = country.IsActive,
                CreatedOn = country.CreatedOn,
                CreatedBy = country.CreatedBy,
                ModifyOn = country.ModifyOn,
                ModifyBy = country.ModifyBy
            });
        }

        public async Task<CountryDto?> GetByIdAsync(long countryId)
        {
            var country = await _countryRepository.GetByIdAsync(countryId);

            if (country == null)
            {
                return null;
            }

            return new CountryDto
            {
                CountryId = country.CountryId,
                RegionId = country.RegionId,
                CountryName = country.CountryName,
                CountryCode = country.CountryCode,
                Description = country.Description,
                IsActive = country.IsActive,
                CreatedOn = country.CreatedOn,
                CreatedBy = country.CreatedBy,
                ModifyOn = country.ModifyOn,
                ModifyBy = country.ModifyBy
            };
        }

        public async Task<long> CreateAsync(CountryDto dto)
        {
            var country = new Country
            {
                RegionId = dto.RegionId,
                CountryName = dto.CountryName.Trim(),
                CountryCode = dto.CountryCode.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = true,
                CreatedOn = DateTime.Now,
                CreatedBy = 1,
                ModifyOn = DateTime.Now,
                ModifyBy = 1
            };

            return await _countryRepository.CreateAsync(country);
        }

        public async Task<bool> UpdateAsync(CountryDto dto)
        {
            var country = new Country
            {
                CountryId = dto.CountryId,
                RegionId = dto.RegionId,
                CountryName = dto.CountryName.Trim(),
                CountryCode = dto.CountryCode.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = dto.IsActive,
                ModifyOn = DateTime.Now,
                ModifyBy = 1
            };

            return await _countryRepository.UpdateAsync(country);
        }

        public async Task<bool> DeleteAsync(long countryId)
        {
            return await _countryRepository.DeleteAsync(countryId);
        }
    }
}
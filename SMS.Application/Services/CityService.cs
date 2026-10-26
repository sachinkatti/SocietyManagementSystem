using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<IEnumerable<CityDto>> GetAllAsync()
        {
            var cities = await _cityRepository.GetAllAsync();

            return cities.Select(MapToDto);
        }

        public async Task<CityDto?> GetByIdAsync(long cityId)
        {
            var city = await _cityRepository.GetByIdAsync(cityId);

            return city == null
                ? null
                : MapToDto(city);
        }

        public async Task<long> CreateAsync(CityDto dto)
        {
            var city = new City
            {
                TalukId = dto.TalukId,
                CityName = dto.CityName.Trim(),
                CityCode = dto.CityCode.Trim(),
                PostalCode = dto.PostalCode?.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = true,
                CreatedOn = DateTime.Now,
                CreatedBy = 1
            };

            return await _cityRepository.CreateAsync(city);
        }

        public async Task<bool> UpdateAsync(CityDto dto)
        {
            var city = new City
            {
                CityId = dto.CityId,
                TalukId = dto.TalukId,
                CityName = dto.CityName.Trim(),
                CityCode = dto.CityCode.Trim(),
                PostalCode = dto.PostalCode?.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = dto.IsActive,
                ModifyOn = DateTime.Now,
                ModifyBy = 1
            };

            return await _cityRepository.UpdateAsync(city);
        }

        public async Task<bool> DeleteAsync(long cityId)
        {
            return await _cityRepository.DeleteAsync(cityId);
        }

        private static CityDto MapToDto(City city)
        {
            return new CityDto
            {
                CityId = city.CityId,
                TalukId = city.TalukId,
                CityName = city.CityName,
                CityCode = city.CityCode,
                PostalCode = city.PostalCode,
                Description = city.Description,
                IsActive = city.IsActive,
                CreatedOn = city.CreatedOn,
                CreatedBy = city.CreatedBy,
                ModifyOn = city.ModifyOn,
                ModifyBy = city.ModifyBy
            };
        }
    }
}
using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class DistrictService : IDistrictService
    {
        private readonly IDistrictRepository _districtRepository;

        public DistrictService(IDistrictRepository districtRepository)
        {
            _districtRepository = districtRepository;
        }

        public async Task<IEnumerable<DistrictDto>> GetAllAsync()
        {
            var districts = await _districtRepository.GetAllAsync();

            return districts.Select(MapToDto);
        }

        public async Task<DistrictDto?> GetByIdAsync(long districtId)
        {
            var district =
                await _districtRepository.GetByIdAsync(districtId);

            return district == null
                ? null
                : MapToDto(district);
        }

        public async Task<long> CreateAsync(DistrictDto dto)
        {
            var district = new District
            {
                StateId = dto.StateId,
                DistrictName = dto.DistrictName.Trim(),
                DistrictCode = dto.DistrictCode.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = true,
                CreatedOn = DateTime.Now,
                CreatedBy = 1
            };

            return await _districtRepository.CreateAsync(district);
        }

        public async Task<bool> UpdateAsync(DistrictDto dto)
        {
            var district = new District
            {
                DistrictId = dto.DistrictId,
                StateId = dto.StateId,
                DistrictName = dto.DistrictName.Trim(),
                DistrictCode = dto.DistrictCode.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = dto.IsActive,
                ModifyOn = DateTime.Now,
                ModifyBy = 1
            };

            return await _districtRepository.UpdateAsync(district);
        }

        public async Task<bool> DeleteAsync(long districtId)
        {
            return await _districtRepository.DeleteAsync(districtId);
        }

        private static DistrictDto MapToDto(District district)
        {
            return new DistrictDto
            {
                DistrictId = district.DistrictId,
                StateId = district.StateId,
                DistrictName = district.DistrictName,
                DistrictCode = district.DistrictCode,
                Description = district.Description,
                IsActive = district.IsActive,
                CreatedOn = district.CreatedOn,
                CreatedBy = district.CreatedBy,
                ModifyOn = district.ModifyOn,
                ModifyBy = district.ModifyBy
            };
        }
    }
}
using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;

        public RegionService(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        public async Task<IEnumerable<RegionDto>> GetAllAsync()
        {
            var regions = await _regionRepository.GetAllAsync();

            return regions.Select(MapToDto);
        }

        public async Task<RegionDto?> GetByIdAsync(long regionId)
        {
            var region = await _regionRepository.GetByIdAsync(regionId);

            return region == null ? null : MapToDto(region);
        }

        public async Task<long> CreateAsync(RegionDto dto)
        {
            var region = new Region
            {
                RegionName = dto.RegionName.Trim(),
                RegionCode = dto.RegionCode.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = true,
                CreatedOn = DateTime.Now,
                CreatedBy = 1,
                ModifyOn = DateTime.Now,
                ModifyBy = 1
            };

            return await _regionRepository.CreateAsync(region);
        }

        public async Task<bool> UpdateAsync(RegionDto dto)
        {
            var region = new Region
            {
                RegionId = dto.RegionId,
                RegionName = dto.RegionName.Trim(),
                RegionCode = dto.RegionCode.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = dto.IsActive,
                ModifyOn = DateTime.Now,
                ModifyBy = 1
            };

            return await _regionRepository.UpdateAsync(region);
        }

        public async Task<bool> DeleteAsync(long regionId)
        {
            return await _regionRepository.DeleteAsync(regionId);
        }

        private static RegionDto MapToDto(Region region)
        {
            return new RegionDto
            {
                RegionId = region.RegionId,
                RegionName = region.RegionName,
                RegionCode = region.RegionCode,
                Description = region.Description,
                IsActive = region.IsActive,
                CreatedOn = region.CreatedOn,
                CreatedBy = region.CreatedBy,
                ModifyOn = region.ModifyOn,
                ModifyBy = region.ModifyBy
            };
        }
    }
}
using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class WingService : IWingService
    {
        private readonly IWingRepository _wingRepository;

        public WingService(IWingRepository wingRepository)
        {
            _wingRepository = wingRepository;
        }

        private static WingDto MapToDto(Wing wing)
        {
            return new WingDto
            {
                WingId = wing.WingId,
                TenantId = wing.TenantId,
                WingName = wing.WingName,
                TotalFloors = wing.TotalFloors,
                Description = wing.Description,
                IsActive = wing.IsActive,
                CreatedOn = wing.CreatedOn,
                CreatedBy = wing.CreatedBy,
                ModifyOn = wing.ModifyOn,
                ModifyBy = wing.ModifyBy
            };
        }

        private static Wing MapToEntity(WingDto dto)
        {
            return new Wing
            {
                WingId = dto.WingId,
                TenantId = dto.TenantId,
                WingName = dto.WingName,
                TotalFloors = dto.TotalFloors,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedOn = dto.CreatedOn,
                CreatedBy = dto.CreatedBy,
                ModifyOn = dto.ModifyOn,
                ModifyBy = dto.ModifyBy
            };
        }

        public async Task<IEnumerable<WingDto>> GetAllAsync()
        {
            var wings = await _wingRepository.GetAllAsync();
            return wings.Select(MapToDto);
        }

        public async Task<WingDto?> GetByIdAsync(long wingId)
        {
            var wing = await _wingRepository.GetByIdAsync(wingId);

            return wing == null ? null : MapToDto(wing);
        }

        public async Task<long> CreateAsync(WingDto dto)
        {
            var wing = MapToEntity(dto);
            wing.IsActive = true;

            return await _wingRepository.CreateAsync(wing);
        }

        public async Task<bool> UpdateAsync(WingDto dto)
        {
            var wing = MapToEntity(dto);

            return await _wingRepository.UpdateAsync(wing);
        }

        public async Task<bool> DeleteAsync(long wingId)
        {
            return await _wingRepository.DeleteAsync(wingId);
        }
    }
}

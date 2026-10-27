using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class AmenityService : IAmenityService
    {
        private readonly IAmenityRepository _amenityRepository;

        public AmenityService(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        public async Task<IEnumerable<AmenityDto>> GetAllAsync()
        {
            var amenities = await _amenityRepository.GetAllAsync();

            return amenities.Select(MapToDto);
        }

        public async Task<AmenityDto?> GetByIdAsync(long amenityId)
        {
            var amenity = await _amenityRepository.GetByIdAsync(amenityId);

            return amenity == null ? null : MapToDto(amenity);
        }

        public async Task<long> CreateAsync(AmenityDto dto)
        {
            var amenity = MapToEntity(dto);

            return await _amenityRepository.CreateAsync(amenity);
        }

        public async Task<bool> UpdateAsync(AmenityDto dto)
        {
            var amenity = MapToEntity(dto);
            amenity.AmenityId = dto.AmenityId;
            amenity.ModifyBy = dto.ModifyBy;

            return await _amenityRepository.UpdateAsync(amenity);
        }

        public async Task<bool> DeleteAsync(long amenityId)
        {
            return await _amenityRepository.DeleteAsync(amenityId);
        }

        private static AmenityDto MapToDto(Amenity amenity)
        {
            return new AmenityDto
            {
                AmenityId = amenity.AmenityId,
                TenantId = amenity.TenantId,
                TenantName = amenity.TenantName,
                Name = amenity.Name,
                DepositRequired = amenity.DepositRequired,
                MaxSlotHours = amenity.MaxSlotHours,
                Description = amenity.Description,
                IsActive = amenity.IsActive,
                CreatedOn = amenity.CreatedOn,
                CreatedBy = amenity.CreatedBy,
                ModifyOn = amenity.ModifyOn,
                ModifyBy = amenity.ModifyBy
            };
        }

        private static Amenity MapToEntity(AmenityDto dto)
        {
            return new Amenity
            {
                AmenityId = dto.AmenityId,
                TenantId = dto.TenantId,
                Name = dto.Name.Trim(),
                DepositRequired = dto.DepositRequired,
                MaxSlotHours = dto.MaxSlotHours,
                Description = string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim(),
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy,
                ModifyBy = dto.ModifyBy
            };
        }
    }
}
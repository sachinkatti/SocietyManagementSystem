
using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class AmenityService : IAmenityService
    {
        private readonly IAmenityRepository _repository;

        public AmenityService(IAmenityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AmenityDto>> GetAllAsync()
        {
            var amenities = await _repository.GetAllAsync();

            return amenities.Select(MapToDto);
        }

        public async Task<AmenityDto?> GetByIdAsync(long amenityId)
        {
            var amenity = await _repository.GetByIdAsync(amenityId);

            return amenity == null ? null : MapToDto(amenity);
        }

        public async Task<long> CreateAsync(AmenityDto dto)
        {
            var amenity = new Amenity
            {
                Name = dto.Name.Trim(),
                DepositRequired = dto.DepositRequired,
                MaxSlotHours = dto.MaxSlotHours,
                Description = dto.Description,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy,
                TenantId = dto.TenantId
            };

            return await _repository.CreateAsync(amenity);
        }

        public async Task<bool> UpdateAsync(AmenityDto dto)
        {
            var amenity = new Amenity
            {
                AmenityId = dto.AmenityId,
                Name = dto.Name.Trim(),
                DepositRequired = dto.DepositRequired,
                MaxSlotHours = dto.MaxSlotHours,
                Description = dto.Description,
                ModifyOn = DateTime.UtcNow,
                ModifyBy = dto.ModifyBy,
                TenantId = dto.TenantId
            };

            return await _repository.UpdateAsync(amenity);
        }

        public Task<bool> DeleteAsync(long amenityId)
        {
            return _repository.DeleteAsync(amenityId);
        }

        private static AmenityDto MapToDto(Amenity amenity)
        {
            return new AmenityDto
            {
                AmenityId = amenity.AmenityId,
                Name = amenity.Name,
                DepositRequired = amenity.DepositRequired,
                MaxSlotHours = amenity.MaxSlotHours,
                Description = amenity.Description,
                IsActive = amenity.IsActive,
                CreatedOn = amenity.CreatedOn,
                CreatedBy = amenity.CreatedBy,
                ModifyOn = amenity.ModifyOn,
                ModifyBy = amenity.ModifyBy,
                TenantId = amenity.TenantId
            };
        }
    }
}

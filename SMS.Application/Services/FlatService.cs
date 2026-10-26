using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class FlatService : IFlatService
    {
        private readonly IFlatRepository _flatRepository;

        public FlatService(IFlatRepository flatRepository)
        {
            _flatRepository = flatRepository;
        }

        private static FlatDto MapToDto(Flat flat)
        {
            return new FlatDto
            {
                FlatId = flat.FlatId,
                WingId = flat.WingId,
                FlatNumber = flat.FlatNumber,
                FloorNumber = flat.FloorNumber,
                SquareFeet = flat.SquareFeet,
                OccupancyStatus = flat.OccupancyStatus,
                ParkingSlotNumber = flat.ParkingSlotNumber,
                Description = flat.Description,
                IsActive = flat.IsActive,
                CreatedOn = flat.CreatedOn,
                CreatedBy = flat.CreatedBy,
                ModifyOn = flat.ModifyOn,
                ModifyBy = flat.ModifyBy
            };
        }

        private static Flat MapToEntity(FlatDto dto)
        {
            return new Flat
            {
                FlatId = dto.FlatId,
                WingId = dto.WingId,
                FlatNumber = dto.FlatNumber,
                FloorNumber = dto.FloorNumber,
                SquareFeet = dto.SquareFeet,
                OccupancyStatus = dto.OccupancyStatus,
                ParkingSlotNumber = dto.ParkingSlotNumber,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedOn = dto.CreatedOn,
                CreatedBy = dto.CreatedBy,
                ModifyOn = dto.ModifyOn,
                ModifyBy = dto.ModifyBy
            };
        }

        public async Task<IEnumerable<FlatDto>> GetAllAsync()
        {
            var flats = await _flatRepository.GetAllAsync();
            return flats.Select(MapToDto);
        }

        public async Task<FlatDto?> GetByIdAsync(long flatId)
        {
            var flat = await _flatRepository.GetByIdAsync(flatId);

            return flat == null ? null : MapToDto(flat);
        }

        public async Task<long> CreateAsync(FlatDto dto)
        {
            var flat = MapToEntity(dto);
            flat.IsActive = true;

            return await _flatRepository.CreateAsync(flat);
        }

        public async Task<bool> UpdateAsync(FlatDto dto)
        {
            var flat = MapToEntity(dto);

            return await _flatRepository.UpdateAsync(flat);
        }

        public async Task<bool> DeleteAsync(long flatId)
        {
            return await _flatRepository.DeleteAsync(flatId);
        }
    }
}
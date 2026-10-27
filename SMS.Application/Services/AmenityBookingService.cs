using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class AmenityBookingService : IAmenityBookingService
    {
        private readonly IAmenityBookingRepository _repository;

        public AmenityBookingService(
            IAmenityBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AmenityBookingDto>> GetAllAsync()
        {
            var bookings = await _repository.GetAllAsync();

            return bookings.Select(MapToDto).ToList();
        }

        public async Task<AmenityBookingDto?> GetByIdAsync(long bookingId)
        {
            var booking = await _repository.GetByIdAsync(bookingId);

            return booking == null ? null : MapToDto(booking);
        }

        public async Task<long> CreateAsync(AmenityBookingDto dto)
        {
            var booking = MapToEntity(dto);

            return await _repository.CreateAsync(booking);
        }

        public async Task<bool> UpdateAsync(AmenityBookingDto dto)
        {
            var booking = MapToEntity(dto);

            booking.BookingId = dto.BookingId;
            booking.ModifyBy = dto.ModifyBy;

            return await _repository.UpdateAsync(booking);
        }

        public async Task<bool> DeleteAsync(
            long bookingId, long? modifyBy = null)
        {
            return await _repository.DeleteAsync(bookingId, modifyBy);
        }

        public async Task<IEnumerable<AmenityBookingAmenityOption>>
            GetAmenitiesByTenantAsync(long tenantId)
        {
            return await _repository.GetAmenitiesByTenantAsync(tenantId);
        }

        public async Task<IEnumerable<AmenityBookingFlatOption>>
            GetFlatsByTenantAsync(long tenantId)
        {
            return await _repository.GetFlatsByTenantAsync(tenantId);
        }

        private static AmenityBookingDto MapToDto(AmenityBooking booking)
        {
            return new AmenityBookingDto
            {
                BookingId = booking.BookingId,

                TenantId = booking.TenantId,
                TenantName = booking.TenantName,

                AmenityId = booking.AmenityId,
                AmenityName = booking.AmenityName,

                FlatId = booking.FlatId,
                FlatNumber = booking.FlatNumber,
                WingName = booking.WingName,

                SlotDate = booking.SlotDate,
                SlotTime = booking.SlotTime,
                DepositPaid = booking.DepositPaid,
                Status = booking.Status,
                Description = booking.Description,

                IsActive = booking.IsActive,

                CreatedOn = booking.CreatedOn,
                CreatedBy = booking.CreatedBy,
                ModifyOn = booking.ModifyOn,
                ModifyBy = booking.ModifyBy
            };
        }

        private static AmenityBooking MapToEntity(AmenityBookingDto dto)
        {
            return new AmenityBooking
            {
                BookingId = dto.BookingId,

                TenantId = dto.TenantId,
                AmenityId = dto.AmenityId,
                FlatId = dto.FlatId,

                SlotDate = dto.SlotDate,
                SlotTime = dto.SlotTime,
                DepositPaid = dto.DepositPaid,
                Status = dto.Status,
                Description = dto.Description,

                IsActive = dto.IsActive,

                CreatedOn = dto.CreatedOn,
                CreatedBy = dto.CreatedBy,
                ModifyOn = dto.ModifyOn,
                ModifyBy = dto.ModifyBy
            };
        }
    }
}
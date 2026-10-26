
using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class AmenityBookingService : IAmenityBookingService
    {
        private readonly IAmenityBookingRepository _repository;

        public AmenityBookingService(IAmenityBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AmenityBookingDto>> GetAllAsync()
        {
            var bookings = await _repository.GetAllAsync();

            return bookings.Select(MapToDto);
        }

        public async Task<AmenityBookingDto?> GetByIdAsync(long bookingId)
        {
            var booking = await _repository.GetByIdAsync(bookingId);

            return booking == null ? null : MapToDto(booking);
        }

        public async Task<long> CreateAsync(AmenityBookingDto dto)
        {
            var booking = new AmenityBooking
            {
                AmenityId = dto.AmenityId,
                FlatId = dto.FlatId,
                SlotDate = dto.SlotDate.Date,
                SlotTime = dto.SlotTime.Trim(),
                DepositPaid = dto.DepositPaid,
                Status = dto.Status.Trim(),
                Description = dto.Description,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy,
                TenantId = dto.TenantId
            };

            return await _repository.CreateAsync(booking);
        }

        public async Task<bool> UpdateAsync(AmenityBookingDto dto)
        {
            var booking = new AmenityBooking
            {
                BookingId = dto.BookingId,
                AmenityId = dto.AmenityId,
                FlatId = dto.FlatId,
                SlotDate = dto.SlotDate.Date,
                SlotTime = dto.SlotTime.Trim(),
                DepositPaid = dto.DepositPaid,
                Status = dto.Status.Trim(),
                Description = dto.Description,
                TenantId = dto.TenantId,
                ModifyOn = DateTime.UtcNow,
                ModifyBy = dto.ModifyBy
            };

            return await _repository.UpdateAsync(booking);
        }

        public Task<bool> DeleteAsync(long bookingId)
        {
            return _repository.DeleteAsync(bookingId);
        }

        private static AmenityBookingDto MapToDto(AmenityBooking booking)
        {
            return new AmenityBookingDto
            {
                BookingId = booking.BookingId,
                AmenityId = booking.AmenityId,
                FlatId = booking.FlatId,
                SlotDate = booking.SlotDate,
                SlotTime = booking.SlotTime,
                DepositPaid = booking.DepositPaid,
                Status = booking.Status,
                Description = booking.Description,
                IsActive = booking.IsActive,
                CreatedOn = booking.CreatedOn,
                CreatedBy = booking.CreatedBy,
                ModifyOn = booking.ModifyOn,
                ModifyBy = booking.ModifyBy,
                TenantId = booking.TenantId
            };
        }
    }
}

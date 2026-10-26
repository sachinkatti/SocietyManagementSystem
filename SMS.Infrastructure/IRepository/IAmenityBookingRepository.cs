
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IAmenityBookingRepository
    {
        Task<IEnumerable<AmenityBooking>> GetAllAsync();
        Task<AmenityBooking?> GetByIdAsync(long bookingId);
        Task<long> CreateAsync(AmenityBooking booking);
        Task<bool> UpdateAsync(AmenityBooking booking);
        Task<bool> DeleteAsync(long bookingId);
    }
}

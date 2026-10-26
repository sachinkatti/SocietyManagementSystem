
namespace SMS.Application
{
    public interface IAmenityBookingService
    {
        Task<IEnumerable<AmenityBookingDto>> GetAllAsync();
        Task<AmenityBookingDto?> GetByIdAsync(long bookingId);
        Task<long> CreateAsync(AmenityBookingDto dto);
        Task<bool> UpdateAsync(AmenityBookingDto dto);
        Task<bool> DeleteAsync(long bookingId);
    }
}


using SMS.Infrastructure;

namespace SMS.Application
{
    public interface IAmenityBookingService
    {
        Task<IEnumerable<AmenityBookingDto>> GetAllAsync();

        Task<AmenityBookingDto?> GetByIdAsync(long bookingId);

        Task<long> CreateAsync(AmenityBookingDto booking);

        Task<bool> UpdateAsync(AmenityBookingDto booking);

        Task<bool> DeleteAsync(long bookingId, long? modifyBy = null);

        Task<IEnumerable<AmenityBookingAmenityOption>>
            GetAmenitiesByTenantAsync(long tenantId);

        Task<IEnumerable<AmenityBookingFlatOption>>
            GetFlatsByTenantAsync(long tenantId);
    }
}

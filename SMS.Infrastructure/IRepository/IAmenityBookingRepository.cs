
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IAmenityBookingRepository
    {
        Task<IEnumerable<AmenityBooking>> GetAllAsync();

        Task<AmenityBooking?> GetByIdAsync(long bookingId);

        Task<long> CreateAsync(AmenityBooking booking);

        Task<bool> UpdateAsync(AmenityBooking booking);

        Task<bool> DeleteAsync(long bookingId, long? modifyBy = null);

        Task<IEnumerable<AmenityBookingAmenityOption>>
            GetAmenitiesByTenantAsync(long tenantId);

        Task<IEnumerable<AmenityBookingFlatOption>>
            GetFlatsByTenantAsync(long tenantId);
    }

    public class AmenityBookingAmenityOption
    {
        public long AmenityId { get; set; }

        public string AmenityName { get; set; } = string.Empty;
    }

    public class AmenityBookingFlatOption
    {
        public long FlatId { get; set; }

        public string FlatNumber { get; set; } = string.Empty;

        public long WingId { get; set; }

        public string WingName { get; set; } = string.Empty;
    }
}

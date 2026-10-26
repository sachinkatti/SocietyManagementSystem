
namespace SMS.Domain
{
    public class AmenityBooking : BaseEntity
    {
        public long BookingId { get; set; }
        public long AmenityId { get; set; }
        public long FlatId { get; set; }
        public DateTime SlotDate { get; set; }
        public string SlotTime { get; set; } = string.Empty;
        public decimal DepositPaid { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long TenantId { get; set; }
    }
}

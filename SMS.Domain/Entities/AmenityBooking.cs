
namespace SMS.Domain
{
    public class AmenityBooking : BaseEntity
    {
        public long BookingId { get; set; }

        public long AmenityId { get; set; }
        public string AmenityName { get; set; } = string.Empty;

        public long FlatId { get; set; }
        public string FlatNumber { get; set; } = string.Empty;
        public string WingName { get; set; } = string.Empty;

        public DateTime SlotDate { get; set; }
        public string SlotTime { get; set; } = string.Empty;

        public decimal DepositPaid { get; set; }

        public string Status { get; set; } = "Booked";

        public string? Description { get; set; }

        public long TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
    }
}

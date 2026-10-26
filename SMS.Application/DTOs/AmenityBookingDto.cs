
namespace SMS.Application
{
    public class AmenityBookingDto
    {
        public long BookingId { get; set; }
        public long AmenityId { get; set; }
        public long FlatId { get; set; }
        public DateTime SlotDate { get; set; }
        public string SlotTime { get; set; } = string.Empty;
        public decimal DepositPaid { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
        public long TenantId { get; set; }

        public string? AmenityName { get; set; }
        public string? FlatNumber { get; set; }
    }
}

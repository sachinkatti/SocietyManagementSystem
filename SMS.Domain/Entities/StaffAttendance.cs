
namespace SMS.Domain
{
    public class StaffAttendance : BaseEntity
    {
        public long AttendanceId { get; set; }

        public string StaffName { get; set; } = string.Empty;

        public string StaffRole { get; set; } = string.Empty;

        public long FlatId { get; set; }

        // Display fields from Flats and Wings tables
        public string? FlatNumber { get; set; }

        public string? WingName { get; set; }

        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public long TenantId { get; set; }

        // Display field from Tenant table
        public string? TenantName { get; set; }
    }
}

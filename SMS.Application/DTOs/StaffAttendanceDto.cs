
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class StaffAttendanceDto
    {
        public long AttendanceId { get; set; }

        [Required(ErrorMessage = "Please enter staff name.")]
        [StringLength(100, ErrorMessage = "Staff name cannot exceed 100 characters.")]
        public string StaffName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter staff role.")]
        [StringLength(50, ErrorMessage = "Staff role cannot exceed 50 characters.")]
        public string StaffRole { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a flat.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid flat.")]
        public long FlatId { get; set; }

        public string? FlatNumber { get; set; }

        public string? WingName { get; set; }

        [Required(ErrorMessage = "Please select check-in time.")]
        public DateTime CheckInTime { get; set; } = DateTime.Now;

        public DateTime? CheckOutTime { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }

        [Required(ErrorMessage = "Please select a society.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid society.")]
        public long TenantId { get; set; }

        public string? TenantName { get; set; }
    }
}

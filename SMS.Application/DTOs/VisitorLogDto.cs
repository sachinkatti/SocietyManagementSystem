
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class VisitorLogDto
    {
        public long VisitorId { get; set; }

        [Required(ErrorMessage = "Visitor name is required.")]
        [StringLength(100, ErrorMessage = "Visitor name cannot exceed 100 characters.")]
        public string VisitorName { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string? PhoneNumber { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Please select a flat.")]
        public long VisitingFlatId { get; set; }

        public string? FlatNumber { get; set; }

        public string? WingName { get; set; }

        [Required(ErrorMessage = "Purpose is required.")]
        [StringLength(50, ErrorMessage = "Purpose cannot exceed 50 characters.")]
        public string Purpose { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Vehicle number cannot exceed 20 characters.")]
        public string? VehicleNumber { get; set; }

        [Required(ErrorMessage = "Approval method is required.")]
        [StringLength(20, ErrorMessage = "Approval method cannot exceed 20 characters.")]
        public string ApprovalMethod { get; set; } = string.Empty;

        [Required(ErrorMessage = "Entry time is required.")]
        public DateTime EntryTime { get; set; } = DateTime.Now;

        public DateTime? ExitTime { get; set; }

        public long? LoggedByGuardId { get; set; }

        public long? GuardUserId { get; set; }

        public string? GuardName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }

        public long? TenantId { get; set; }

        public string? TenantName { get; set; }
    }
}

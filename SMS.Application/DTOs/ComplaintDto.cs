
using System;
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class ComplaintDto
    {
        public long ComplaintId { get; set; }

        [Required(ErrorMessage = "Please select a resident.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid resident.")]
        public long ResidentId { get; set; }

        public string? ResidentName { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Photo URL cannot exceed 250 characters.")]
        public string? PhotoUrl { get; set; }

        [Required(ErrorMessage = "Priority is required.")]
        [RegularExpression("^(Low|Medium|High)$",
            ErrorMessage = "Priority must be Low, Medium, or High.")]
        public string Priority { get; set; } = "Medium";

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Open|In Progress|Resolved|Closed|Rejected)$",
            ErrorMessage = "Please select a valid complaint status.")]
        public string Status { get; set; } = "Open";

        public DateTime DateRaised { get; set; } = DateTime.Now;

        public DateTime? DateResolved { get; set; }

        [StringLength(500, ErrorMessage = "Resolution notes cannot exceed 500 characters.")]
        public string? ResolutionNotes { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? ResidentRating { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid tenant.")]
        public long TenantId { get; set; }

        public string? TenantName { get; set; }
    }
}

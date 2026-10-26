
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class WingDto
    {
        public long WingId { get; set; }

        [Required(ErrorMessage = "Please select a tenant.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid tenant.")]
        public long TenantId { get; set; }

        [Required(ErrorMessage = "Wing name is required.")]
        [StringLength(100, ErrorMessage = "Wing name cannot exceed 100 characters.")]
        public string WingName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Total floors must be at least 1.")]
        public int TotalFloors { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

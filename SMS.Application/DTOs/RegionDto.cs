using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class RegionDto
    {
        public long RegionId { get; set; }

        [Required(ErrorMessage = "Region name is required.")]
        [StringLength(100, ErrorMessage = "Region name cannot exceed 100 characters.")]
        public string RegionName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region code is required.")]
        [StringLength(20, ErrorMessage = "Region code cannot exceed 20 characters.")]
        public string RegionCode { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}
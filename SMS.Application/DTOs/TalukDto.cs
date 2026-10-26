using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class TalukDto
    {
        public long TalukId { get; set; }

        [Required(ErrorMessage = "District is required.")]
        public long DistrictId { get; set; }

        public string? DistrictName { get; set; }

        [Required(ErrorMessage = "Taluk name is required.")]
        [StringLength(100, ErrorMessage = "Taluk name cannot exceed 100 characters.")]
        public string TalukName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Taluk code is required.")]
        [StringLength(20, ErrorMessage = "Taluk code cannot exceed 20 characters.")]
        public string TalukCode { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}
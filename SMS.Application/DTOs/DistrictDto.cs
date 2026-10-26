using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class DistrictDto
    {
        public long DistrictId { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public long StateId { get; set; }

        public string? StateName { get; set; }

        [Required(ErrorMessage = "District name is required.")]
        [StringLength(100, ErrorMessage = "District name cannot exceed 100 characters.")]
        public string DistrictName { get; set; } = string.Empty;

        [Required(ErrorMessage = "District code is required.")]
        [StringLength(20, ErrorMessage = "District code cannot exceed 20 characters.")]
        public string DistrictCode { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}
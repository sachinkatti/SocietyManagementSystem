using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class CountryDto
    {
        public long CountryId { get; set; }

        [Required(ErrorMessage = "Region is required.")]
        public long RegionId { get; set; }

        public string? RegionName { get; set; }

        [Required(ErrorMessage = "Country name is required.")]
        [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
        public string CountryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country code is required.")]
        [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters.")]
        public string CountryCode { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}
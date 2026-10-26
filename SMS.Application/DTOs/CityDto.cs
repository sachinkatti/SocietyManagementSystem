using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class CityDto
    {
        public long CityId { get; set; }

        [Required(ErrorMessage = "Taluk is required.")]
        public long TalukId { get; set; }

        public string? TalukName { get; set; }

        [Required(ErrorMessage = "City name is required.")]
        [StringLength(100, ErrorMessage = "City name cannot exceed 100 characters.")]
        public string CityName { get; set; } = string.Empty;

        [Required(ErrorMessage = "City code is required.")]
        [StringLength(20, ErrorMessage = "City code cannot exceed 20 characters.")]
        public string CityCode { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters.")]
        public string? PostalCode { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}
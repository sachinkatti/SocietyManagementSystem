using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class StateDto
    {
        public long StateId { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public long CountryId { get; set; }

        public string? CountryName { get; set; }

        [Required(ErrorMessage = "State name is required.")]
        [StringLength(100, ErrorMessage = "State name cannot exceed 100 characters.")]
        public string StateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "State code is required.")]
        [StringLength(20, ErrorMessage = "State code cannot exceed 20 characters.")]
        public string StateCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class ResidentDto
    {
        public long ResidentId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, long.MaxValue)]
        public long UserId { get; set; }

        [Required(ErrorMessage = "Flat ID is required.")]
        [Range(1, long.MaxValue)]
        public long FlatId { get; set; }

        [Required(ErrorMessage = "Resident type is required.")]
        [StringLength(20)]
        public string ResidentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Move-in date is required.")]
        [DataType(DataType.Date)]
        public DateTime MoveInDate { get; set; } = DateTime.Today;

        [StringLength(15)]
        [Display(Name = "Emergency Contact")]
        public string? EmergencyContact { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class ResidentDto
    {
        public long ResidentId { get; set; }

        [Required(ErrorMessage = "Please select a user.")]
        [Range(1, long.MaxValue, ErrorMessage = "Select a valid user.")]
        public long UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a flat.")]
        [Range(1, long.MaxValue, ErrorMessage = "Select a valid flat.")]
        public long FlatId { get; set; }

        public string FlatNumber { get; set; } = string.Empty;
        public string WingName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Resident type is required.")]
        [StringLength(20, ErrorMessage = "Resident type cannot exceed 20 characters.")]
        public string ResidentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Move-in date is required.")]
        [DataType(DataType.Date)]
        public DateTime MoveInDate { get; set; } = DateTime.Today;

        [Phone(ErrorMessage = "Enter a valid emergency contact number.")]
        [StringLength(15, ErrorMessage = "Emergency contact cannot exceed 15 characters.")]
        public string? EmergencyContact { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

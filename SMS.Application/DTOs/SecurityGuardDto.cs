
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class SecurityGuardDto
    {
        public long GuardId { get; set; }

        [Required(ErrorMessage = "Please select a user.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid user.")]
        public long UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shift timing is required.")]
        [StringLength(100)]
        public string ShiftTiming { get; set; } = string.Empty;

        [StringLength(100)]
        public string? AssignedGate { get; set; }

        [Required(ErrorMessage = "Joining date is required.")]
        [DataType(DataType.Date)]
        public DateTime JoiningDate { get; set; } = DateTime.Today;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

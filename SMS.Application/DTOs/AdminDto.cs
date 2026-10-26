using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class AdminDto
    {
        public long AdminId { get; set; }

        [Required(ErrorMessage = "Please select a user.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid user.")]
        public long UserId { get; set; }

        public long TenantId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Designation is required.")]
        [StringLength(200, ErrorMessage = "Designation cannot exceed 200 characters.")]
        public string Designation { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "Contact number cannot exceed 30 characters.")]
        public string? ContactNumber { get; set; }

        [Required(ErrorMessage = "Assignment date is required.")]
        [DataType(DataType.Date)]
        public DateTime DateAssigned { get; set; } = DateTime.Today;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class UserDto
    {
        public long UserId { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid tenant.")]
        public long TenantId { get; set; }

        [Required, StringLength(300)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(300)]
        public string? MiddleName { get; set; }

        [Required, StringLength(300)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(300)]
        public string Email { get; set; } = string.Empty;

        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [StringLength(1000)]
        public string? UserDescription { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }

        public string FullName =>
            $"{FirstName} {MiddleName} {LastName}"
                .Replace("  ", " ").Trim();
    }
}

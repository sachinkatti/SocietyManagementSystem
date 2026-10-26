
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class TenantDto
    {
        public long TenantId { get; set; }

        [Required(ErrorMessage = "Tenant name is required.")]
        [StringLength(150)]
        public string TenantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tenant code is required.")]
        [StringLength(50)]
        public string TenantCode { get; set; } = string.Empty;

        [StringLength(250)]
        public string? TenantAddress { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(256)]
        public string? Email { get; set; }

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? TenantDescription { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}

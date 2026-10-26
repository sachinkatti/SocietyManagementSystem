
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class RoleDto
    {
        public long RoleId { get; set; }

        [Required]
        public long TenantId { get; set; }

        [Required]
        [StringLength(100)]
        public string RoleName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RoleCode { get; set; } = string.Empty;

        [StringLength(500)]
        public string? RoleDescription { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}

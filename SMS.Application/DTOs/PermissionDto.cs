
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class PermissionDto
    {
        public long PermissionId { get; set; }

        [Required]
        [StringLength(100)]
        public string PermissionName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string PermissionCode { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}

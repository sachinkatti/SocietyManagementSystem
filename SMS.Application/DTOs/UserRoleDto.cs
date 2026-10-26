
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class UserRoleDto
    {
        public long UserRoleId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        [Range(1, long.MaxValue)]
        public long RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string RoleCode { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}


namespace SMS.Domain
{
    public class UserRole
    {
        public long UserRoleId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public long RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

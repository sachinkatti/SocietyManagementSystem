namespace SMS.Domain
{
    public class User : BaseEntity
    {
        public long UserId { get; set; }
        public long TenantId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? UserDescription { get; set; }
        public string? PasswordHash { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutEndUtc { get; set; }
        public DateTime? LastLoginOn { get; set; }
        public bool MustChangePassword { get; set; }
    }
}
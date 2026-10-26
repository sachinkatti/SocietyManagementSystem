namespace SMS.Domain
{
    public class Tenant : BaseEntity
    {
        public long TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string TenantCode { get; set; } = string.Empty;
        public string? TenantAddress { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? TenantDescription { get; set; }
    }
}

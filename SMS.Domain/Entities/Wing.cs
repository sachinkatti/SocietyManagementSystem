namespace SMS.Domain
{
    public class Wing : BaseEntity
    {
        public long WingId { get; set; }
        public long TenantId { get; set; }
        public string WingName { get; set; } = string.Empty;
        public int TotalFloors { get; set; }
        public string? Description { get; set; }
    }
}
namespace SMS.Domain
{
    public class VehicleRegistry : BaseEntity
    {
        public long VehicleId { get; set; }
        public long FlatId { get; set; }
        public long TenantId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
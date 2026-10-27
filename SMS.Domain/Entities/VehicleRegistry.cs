
namespace SMS.Domain
{
    public class VehicleRegistry : BaseEntity
    {
        public long VehicleId { get; set; }

        public long TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;

        public long FlatId { get; set; }
        public string FlatNumber { get; set; } = string.Empty;
        public string WingName { get; set; } = string.Empty;

        public string VehicleNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

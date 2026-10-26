
namespace SMS.Domain
{
    public class VisitorLog : BaseEntity
    {
        public long VisitorId { get; set; }
        public string VisitorName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public long VisitingFlatId { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string? VehicleNumber { get; set; }
        public string ApprovalMethod { get; set; } = string.Empty;
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public long? LoggedByGuardId { get; set; }
        public string? Description { get; set; }
        public long? TenantId { get; set; }
    }
}


namespace SMS.Domain
{
    public class SecurityGuard : BaseEntity
    {
        public long GuardId { get; set; }
        public long UserId { get; set; }
        public string ShiftTiming { get; set; } = string.Empty;
        public string? AssignedGate { get; set; }
        public DateTime JoiningDate { get; set; }
    }
}


namespace SMS.Domain
{
    public class SecurityGuard
    {
        public long GuardId { get; set; }
        public long UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ShiftTiming { get; set; } = string.Empty;
        public string? AssignedGate { get; set; }
        public DateTime JoiningDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

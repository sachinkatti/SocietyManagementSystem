
namespace SMS.Domain
{
    public class Resident : BaseEntity
    {
        public long ResidentId { get; set; }
        public long UserId { get; set; }
        public long FlatId { get; set; }
        public string ResidentType { get; set; } = string.Empty;
        public DateTime MoveInDate { get; set; }
        public string? EmergencyContact { get; set; }
    }
}

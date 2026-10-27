
namespace SMS.Domain
{
    public class Resident : BaseEntity
    {
        public long ResidentId { get; set; }

        public long UserId { get; set; }
        public string FullName { get; set; } = string.Empty;

        public long FlatId { get; set; }
        public string FlatNumber { get; set; } = string.Empty;
        public string WingName { get; set; } = string.Empty;

        public string ResidentType { get; set; } = string.Empty;

        public DateTime MoveInDate { get; set; }

        public string? EmergencyContact { get; set; }
    }
}

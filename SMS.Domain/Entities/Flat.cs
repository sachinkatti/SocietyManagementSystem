namespace SMS.Domain
{
    public class Flat : BaseEntity
    {
        public long FlatId { get; set; }

        public long WingId { get; set; }

        public string WingName { get; set; } = string.Empty;

        public string FlatNumber { get; set; } = string.Empty;

        public int FloorNumber { get; set; }

        public decimal SquareFeet { get; set; }

        public string OccupancyStatus { get; set; } = string.Empty;

        public string? ParkingSlotNumber { get; set; }

        public string? Description { get; set; }
    }
}
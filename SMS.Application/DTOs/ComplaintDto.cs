namespace SMS.Application
{
    public class ComplaintDto
    {
        public long ComplaintId { get; set; }
        public long ResidentId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DateRaised { get; set; }
        public DateTime? DateResolved { get; set; }
        public string? ResolutionNotes { get; set; }
        public int? ResidentRating { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
        public long TenantId { get; set; }

        public string? ResidentName { get; set; }
        public string? FlatNumber { get; set; }
    }
}
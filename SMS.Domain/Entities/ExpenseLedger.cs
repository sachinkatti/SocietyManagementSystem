namespace SMS.Domain
{
    public class ExpenseLedger : BaseEntity
    {
        public long LedgerId { get; set; }
        public string EntryType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime EntryDate { get; set; }
        public long RecordedByAdminId { get; set; }
        public long TenantId { get; set; }
    }
}
namespace SMS.Application
{
    public class ExpenseLedgerDto
    {
        public long LedgerId { get; set; }
        public string EntryType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime EntryDate { get; set; }
        public long RecordedByAdminId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
        public long TenantId { get; set; }
    }
}
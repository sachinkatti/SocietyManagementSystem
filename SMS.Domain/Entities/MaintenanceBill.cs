namespace SMS.Domain
{
    public class MaintenanceBill : BaseEntity
    {
        public long BillId { get; set; }
        public long FlatId { get; set; }
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal PenaltyAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; }
        public string? TransactionRef { get; set; }
        public string? Description { get; set; }
        public long TenantId { get; set; }
    }
}
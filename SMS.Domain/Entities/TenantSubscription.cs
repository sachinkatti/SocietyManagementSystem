namespace SMS.Domain
{
    public class TenantSubscription : BaseEntity
    {
        public long SubscriptionId { get; set; }
        public long TenantId { get; set; }
        public long PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public string? PaymentReference { get; set; }
        public string? Description { get; set; }
    }
}
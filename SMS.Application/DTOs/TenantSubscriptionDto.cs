
namespace SMS.Application
{
    public class TenantSubscriptionDto
    {
        public long SubscriptionId { get; set; }

        public long TenantId { get; set; }

        public string? TenantName { get; set; }

        public long PlanId { get; set; }

        public string? PlanName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; } = "Active";

        public decimal AmountPaid { get; set; }

        public string? PaymentReference { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}

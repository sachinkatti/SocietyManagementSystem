
namespace SMS.Domain
{
    public class Invoice
    {
        public long InvoiceId { get; set; }

        public long SubscriptionId { get; set; }

        public long TenantId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public string Status { get; set; } = "Pending";

        public string? Description { get; set; }
    }
}

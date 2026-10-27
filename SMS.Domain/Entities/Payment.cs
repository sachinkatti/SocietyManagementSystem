
namespace SMS.Domain
{
    public class Payment
    {
        public long PaymentId { get; set; }
        public long InvoiceId { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? TransactionId { get; set; }
        public string? Description { get; set; }

        // Display-only property from the Invoice table
        public string? InvoiceNumber { get; set; }
    }
}

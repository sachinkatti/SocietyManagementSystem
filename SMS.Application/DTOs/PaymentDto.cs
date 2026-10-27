
namespace SMS.Application
{
    public class PaymentDto
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

        // Display-only field for the invoice dropdown and payment list
        public string? InvoiceNumber { get; set; }
    }
}

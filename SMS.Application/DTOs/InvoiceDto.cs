using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class InvoiceDto
    {
        public long InvoiceId { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public long SubscriptionId { get; set; }

        [Required(ErrorMessage = "Tenant is required.")]
        public long TenantId { get; set; }

        [Required(ErrorMessage = "Invoice number is required.")]
        [StringLength(50, ErrorMessage = "Invoice number cannot exceed 50 characters.")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Range(0, 999999999.99, ErrorMessage = "Amount cannot be negative.")]
        public decimal Amount { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Tax amount cannot be negative.")]
        public decimal TaxAmount { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Total amount cannot be negative.")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Invoice date is required.")]
        public DateTime InvoiceDate { get; set; }

        [Required(ErrorMessage = "Due date is required.")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(30, ErrorMessage = "Status cannot exceed 30 characters.")]
        public string Status { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}
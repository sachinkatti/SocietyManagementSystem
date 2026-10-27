using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class MaintenanceBillDto
    {
        public long BillId { get; set; }

        [Required(ErrorMessage = "Please select a flat.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid flat.")]
        public long FlatId { get; set; }

        public string? FlatNumber { get; set; }

        [Required(ErrorMessage = "Billing month is required.")]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int BillingMonth { get; set; }

        [Required(ErrorMessage = "Billing year is required.")]
        [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100.")]
        public int BillingYear { get; set; }

        [Required(ErrorMessage = "Base amount is required.")]
        [Range(typeof(decimal), "0", "9999999999999999.99",
            ErrorMessage = "Base amount cannot be negative.")]
        public decimal BaseAmount { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999.99",
            ErrorMessage = "Penalty amount cannot be negative.")]
        public decimal PenaltyAmount { get; set; }

        public decimal TotalAmount =>
            BaseAmount + PenaltyAmount;

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Payment status is required.")]
        [StringLength(20, ErrorMessage = "Payment status cannot exceed 20 characters.")]
        [RegularExpression(
            "^(Pending|Paid|Overdue|Partially Paid)$",
            ErrorMessage = "Select a valid payment status.")]
        public string PaymentStatus { get; set; } = "Pending";

        [DataType(DataType.Date)]
        public DateTime? PaymentDate { get; set; }

        [StringLength(100, ErrorMessage = "Transaction reference cannot exceed 100 characters.")]
        public string? TransactionRef { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }

        [Required(ErrorMessage = "Tenant is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid tenant.")]
        public long TenantId { get; set; }

        public string? TenantName { get; set; }
    }
}

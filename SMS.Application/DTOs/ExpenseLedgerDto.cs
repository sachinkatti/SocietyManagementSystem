using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class ExpenseLedgerDto
    {
        public long LedgerId { get; set; }

        [Required(ErrorMessage = "Please select an entry type.")]
        [RegularExpression("^(Income|Expense)$",
            ErrorMessage = "Entry type must be Income or Expense.")]
        public string EntryType { get; set; } = "Expense";

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100,
            ErrorMessage = "Category cannot exceed 100 characters.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required.")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99",
            ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [StringLength(500,
            ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Entry date is required.")]
        [DataType(DataType.Date)]
        public DateTime EntryDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please select an administrator.")]
        [Range(1, long.MaxValue,
            ErrorMessage = "Please select a valid administrator.")]
        public long RecordedByAdminId { get; set; }

        public string? RecordedByAdminName { get; set; }

        public string? Designation { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }

        [Required(ErrorMessage = "Tenant is required.")]
        [Range(1, long.MaxValue,
            ErrorMessage = "Please select a valid tenant.")]
        public long TenantId { get; set; }

        public string? TenantName { get; set; }
    }
}

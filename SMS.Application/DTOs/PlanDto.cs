
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class PlanDto
    {
        public long PlanId { get; set; }

        [Required]
        [StringLength(100)]
        public string PlanName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PlanCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string BillingCycle { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

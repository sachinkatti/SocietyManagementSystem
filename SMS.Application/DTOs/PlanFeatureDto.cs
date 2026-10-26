
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class PlanFeatureDto
    {
        public long PlanFeatureId { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Please select a plan.")]
        public long PlanId { get; set; }

        public string PlanName { get; set; } = string.Empty;

        [Range(1, long.MaxValue, ErrorMessage = "Please select a feature.")]
        public long FeatureId { get; set; }

        public string FeatureName { get; set; } = string.Empty;

        public string FeatureCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FeatureValue { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

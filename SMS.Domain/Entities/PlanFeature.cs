
namespace SMS.Domain
{
    public class PlanFeature
    {
        public long PlanFeatureId { get; set; }
        public long PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public long FeatureId { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public string FeatureCode { get; set; } = string.Empty;
        public string FeatureValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

namespace SMS.Domain
{
    public class Feature
    {
        public long FeatureId { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public string FeatureCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}
namespace SMS.Domain
{
    public class Region : BaseEntity
    {
        public long RegionId { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public string RegionCode { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
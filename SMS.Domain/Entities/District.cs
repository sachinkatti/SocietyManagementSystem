namespace SMS.Domain
{
    public class District : BaseEntity
    {
        public long DistrictId { get; set; }
        public long StateId { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public string DistrictCode { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
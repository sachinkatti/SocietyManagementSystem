namespace SMS.Domain
{
    public class Country : BaseEntity
    {
        public long CountryId { get; set; }
        public long RegionId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
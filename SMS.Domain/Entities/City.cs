namespace SMS.Domain
{
    public class City : BaseEntity
    {
        public long CityId { get; set; }
        public long TalukId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string CityCode { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string? Description { get; set; }
    }
}
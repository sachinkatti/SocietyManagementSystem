namespace SMS.Domain
{
    public class Taluk : BaseEntity
    {
        public long TalukId { get; set; }
        public long DistrictId { get; set; }
        public string TalukName { get; set; } = string.Empty;
        public string TalukCode { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
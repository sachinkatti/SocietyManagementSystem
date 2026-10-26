namespace SMS.Domain
{
    public class State : BaseEntity
    {
        public long StateId { get; set; }
        public long CountryId { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string StateCode { get; set; } = string.Empty;
    }
}
namespace SMS.Domain
{
    public class Language : BaseEntity
    {
        public long LanguageId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
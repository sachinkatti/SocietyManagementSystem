
namespace SMS.Domain
{
    public class Notice : BaseEntity
    {
        public long NoticeId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public DateTime PublishDate { get; set; }

        public long PublishedByAdminId { get; set; }

        public long TenantId { get; set; }

        public string? TenantName { get; set; }

        public string? AdminName { get; set; }
    }
}

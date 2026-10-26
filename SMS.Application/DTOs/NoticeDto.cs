namespace SMS.Application
{
    public class NoticeDto
    {
        public long NoticeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public long PublishedByAdminId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
        public long TenantId { get; set; }

        public string? PublishedByAdminName { get; set; }
    }
}
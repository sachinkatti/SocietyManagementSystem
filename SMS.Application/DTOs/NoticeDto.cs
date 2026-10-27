using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class NoticeDto
    {
        public long NoticeId { get; set; }

        [Required(ErrorMessage = "Notice title is required.")]
        [StringLength(300, ErrorMessage = "Title cannot exceed 300 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Notice message is required.")]
        [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters.")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category.")]
        [StringLength(60, ErrorMessage = "Category cannot exceed 60 characters.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Publish date is required.")]
        public DateTime PublishDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Please select a publishing admin.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid admin.")]
        public long PublishedByAdminId { get; set; }

        // Display-only admin name
        public string? AdminName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }

        [Required(ErrorMessage = "Please select a tenant.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid tenant.")]
        public long TenantId { get; set; }

        // Display-only tenant name
        public string? TenantName { get; set; }

        // Admin dropdown options
        public List<NoticeAdminDto> Admins { get; set; } = new();
    }

    public class NoticeAdminDto
    {
        public long AdminId { get; set; }

        public string AdminName { get; set; } = string.Empty;
    }
}
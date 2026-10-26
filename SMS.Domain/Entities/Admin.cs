
namespace SMS.Domain
{
    public class Admin
    {
        public long AdminId { get; set; }
        public long UserId { get; set; }
        public long TenantId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        public string Designation { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public DateTime DateAssigned { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

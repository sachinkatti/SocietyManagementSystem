
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class AmenityDto
    {
        public long AmenityId { get; set; }

        [Required(ErrorMessage = "Please select a tenant.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid tenant.")]
        public long TenantId { get; set; }

        public string TenantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amenity name is required.")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Deposit amount is required.")]
        [Range(typeof(decimal), "0", "9999999999999999.99",
            ErrorMessage = "Deposit must be zero or greater.")]
        public decimal DepositRequired { get; set; }

        [Required(ErrorMessage = "Maximum slot hours is required.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Maximum slot hours must be greater than zero.")]
        public int MaxSlotHours { get; set; }

        [StringLength(1000,
            ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class VehicleRegistryDto
    {
        public long VehicleId { get; set; }

        [Required(ErrorMessage = "Please select a tenant.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid tenant.")]
        public long TenantId { get; set; }

        public string TenantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a flat.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid flat.")]
        public long FlatId { get; set; }

        public string FlatNumber { get; set; } = string.Empty;
        public string WingName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vehicle number is required.")]
        [StringLength(40, ErrorMessage = "Vehicle number cannot exceed 40 characters.")]
        [RegularExpression(
            @"^[A-Za-z0-9 -]+$",
            ErrorMessage = "Enter a valid vehicle number.")]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a vehicle type.")]
        [StringLength(40)]
        public string VehicleType { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

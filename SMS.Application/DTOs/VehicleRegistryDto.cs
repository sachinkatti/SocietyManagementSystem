using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class VehicleRegistryDto
    {
        public long VehicleId { get; set; }

        [Required]
        public long FlatId { get; set; }

        [Required]
        public long TenantId { get; set; }

        [Required, StringLength(40)]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required, StringLength(40)]
        public string VehicleType { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }

        public string FlatNumber { get; set; } = string.Empty;
    }
}
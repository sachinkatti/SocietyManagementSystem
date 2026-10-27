
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class AmenityBookingDto
    {
        public long BookingId { get; set; }

        [Required(ErrorMessage = "Please select a society.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid society.")]
        public long TenantId { get; set; }

        public string TenantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select an amenity.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid amenity.")]
        public long AmenityId { get; set; }

        public string AmenityName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a flat.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid flat.")]
        public long FlatId { get; set; }

        public string FlatNumber { get; set; } = string.Empty;

        public string WingName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a booking date.")]
        [DataType(DataType.Date)]
        public DateTime SlotDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please enter a time slot.")]
        [StringLength(40, ErrorMessage = "Time slot cannot exceed 40 characters.")]
        public string SlotTime { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "9999999999999999.99",
            ErrorMessage = "Deposit must be zero or greater.")]
        public decimal DepositPaid { get; set; }

        [Required(ErrorMessage = "Please select a booking status.")]
        [StringLength(40, ErrorMessage = "Status cannot exceed 40 characters.")]
        public string Status { get; set; } = "Booked";

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}

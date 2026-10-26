
using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class FlatDto
    {
        public long FlatId { get; set; }

        [Required(ErrorMessage = "Please select a wing.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid wing.")]
        public long WingId { get; set; }

        [Required(ErrorMessage = "Flat number is required.")]
        [StringLength(10, ErrorMessage = "Flat number cannot exceed 10 characters.")]
        public string FlatNumber { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Floor number cannot be negative.")]
        public int FloorNumber { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Square feet must be greater than zero.")]
        public decimal SquareFeet { get; set; }

        [Required(ErrorMessage = "Occupancy status is required.")]
        [StringLength(20, ErrorMessage = "Occupancy status cannot exceed 20 characters.")]
        public string OccupancyStatus { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Parking slot number cannot exceed 20 characters.")]
        public string? ParkingSlotNumber { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
    }
}

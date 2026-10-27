using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class FlatDto
    {
        public long FlatId { get; set; }

        [Required(ErrorMessage = "Please select a wing.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid wing.")]
        public long WingId { get; set; }

        public string WingName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Flat number is required.")]
        [StringLength(50, ErrorMessage = "Flat number cannot exceed 50 characters.")]
        public string FlatNumber { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Floor number cannot be negative.")]
        public int FloorNumber { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999.99",
            ErrorMessage = "Square feet must be greater than zero.")]
        public decimal SquareFeet { get; set; }

        [Required(ErrorMessage = "Please select occupancy status.")]
        [StringLength(50)]
        public string OccupancyStatus { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Parking slot cannot exceed 50 characters.")]
        public string? ParkingSlotNumber { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}
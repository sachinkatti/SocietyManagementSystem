
namespace SMS.Domain
{
    public class Amenity : BaseEntity
    {
        public long AmenityId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal DepositRequired { get; set; }

        public int MaxSlotHours { get; set; }

        public string? Description { get; set; }

        public long TenantId { get; set; }

        // Display property from the Tenant table
        public string TenantName { get; set; } = string.Empty;
    }
}
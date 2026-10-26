namespace SMS.Application
{
    public class AmenityDto
    {
        public long AmenityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DepositRequired { get; set; }
        public int MaxSlotHours { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public long? ModifyBy { get; set; }
        public long TenantId { get; set; }
    }
}
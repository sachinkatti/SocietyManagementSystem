
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IComplaintRepository
    {
        Task<List<Complaint>> GetAllAsync();

        Task<Complaint?> GetByIdAsync(long complaintId);

        Task<long> CreateAsync(Complaint complaint);

        Task<bool> UpdateAsync(Complaint complaint);

        Task<bool> DeleteAsync(long complaintId, long? modifyBy);

        Task<List<ComplaintResidentOption>> GetResidentsByTenantAsync(long tenantId);
    }

    public class ComplaintResidentOption
    {
        public long ResidentId { get; set; }

        public string ResidentName { get; set; } = string.Empty;

        public long? FlatId { get; set; }
    }
}

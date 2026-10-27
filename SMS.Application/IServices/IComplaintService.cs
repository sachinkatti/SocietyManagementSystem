
using SMS.Infrastructure;

namespace SMS.Application
{
    public interface IComplaintService
    {
        Task<List<ComplaintDto>> GetAllAsync();

        Task<ComplaintDto?> GetByIdAsync(long complaintId);

        Task<long> CreateAsync(ComplaintDto complaintDto);

        Task<bool> UpdateAsync(ComplaintDto complaintDto);

        Task<bool> DeleteAsync(long complaintId, long? modifyBy);

        Task<List<ComplaintResidentOption>> GetResidentsByTenantAsync(
            long tenantId);
    }
}

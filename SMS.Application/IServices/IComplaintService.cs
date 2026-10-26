namespace SMS.Application
{
    public interface IComplaintService
    {
        Task<List<ComplaintDto>> GetAllAsync();
        Task<ComplaintDto?> GetByIdAsync(long complaintId);
        Task<long> CreateAsync(ComplaintDto dto);
        Task<bool> UpdateAsync(ComplaintDto dto);
        Task<bool> DeleteAsync(long complaintId, long? modifyBy);
    }
}
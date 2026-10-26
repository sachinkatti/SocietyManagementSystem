using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IVisitorLogRepository
    {
        Task<List<VisitorLog>> GetAllAsync();
        Task<VisitorLog?> GetByIdAsync(long visitorId);
        Task<long> CreateAsync(VisitorLog visitorLog);
        Task<bool> UpdateAsync(VisitorLog visitorLog);
        Task<bool> DeleteAsync(long visitorId, long? modifyBy);
    }
}
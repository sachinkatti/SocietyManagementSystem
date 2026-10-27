
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IVisitorLogRepository
    {
        Task<IEnumerable<VisitorLog>> GetAllAsync();

        Task<VisitorLog?> GetByIdAsync(long visitorId);

        Task<long> CreateAsync(VisitorLog visitorLog);

        Task<bool> UpdateAsync(VisitorLog visitorLog);

        Task<bool> DeleteAsync(long visitorId, long? modifyBy = null);

        Task<IEnumerable<VisitorLogFlatOption>> GetFlatsByTenantAsync(long tenantId);

        Task<IEnumerable<VisitorLogGuardOption>> GetGuardsByTenantAsync(long tenantId);
    }

    public class VisitorLogFlatOption
    {
        public long FlatId { get; set; }

        public string FlatNumber { get; set; } = string.Empty;

        public long WingId { get; set; }

        public string WingName { get; set; } = string.Empty;
    }

    public class VisitorLogGuardOption
    {
        public long GuardId { get; set; }

        public long UserId { get; set; }

        public string GuardName { get; set; } = string.Empty;
    }
}

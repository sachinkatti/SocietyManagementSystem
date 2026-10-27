
using SMS.Infrastructure;

namespace SMS.Application
{
    public interface IVisitorLogService
    {
        Task<IEnumerable<VisitorLogDto>> GetAllAsync();

        Task<VisitorLogDto?> GetByIdAsync(long visitorId);

        Task<long> CreateAsync(VisitorLogDto visitorLogDto);

        Task<bool> UpdateAsync(VisitorLogDto visitorLogDto);

        Task<bool> DeleteAsync(long visitorId, long? modifyBy = null);

        Task<IEnumerable<VisitorLogFlatOption>> GetFlatsByTenantAsync(
            long tenantId);

        Task<IEnumerable<VisitorLogGuardOption>> GetGuardsByTenantAsync(
            long tenantId);
    }
}

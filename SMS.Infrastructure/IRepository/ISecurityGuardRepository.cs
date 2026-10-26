
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface ISecurityGuardRepository
    {
        Task<IEnumerable<SecurityGuard>> GetAllAsync();
        Task<SecurityGuard?> GetByIdAsync(long guardId);
        Task<long> CreateAsync(SecurityGuard guard);
        Task<bool> UpdateAsync(SecurityGuard guard);
        Task<bool> DeleteAsync(long guardId);
    }
}

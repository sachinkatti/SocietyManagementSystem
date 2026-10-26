using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface ITenantRepository
    {
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant?> GetByIdAsync(long tenantId);
        Task<long> CreateAsync(Tenant tenant);
        Task<bool> UpdateAsync(Tenant tenant);
        Task<bool> DeleteAsync(long tenantId);
    }
}
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface ITenantSubscriptionRepository
    {
        Task<IEnumerable<TenantSubscription>> GetAllAsync();
        Task<TenantSubscription?> GetByIdAsync(long subscriptionId);
        Task<long> CreateAsync(TenantSubscription entity);
        Task<bool> UpdateAsync(TenantSubscription entity);
        Task<bool> DeleteAsync(long subscriptionId);
    }
}
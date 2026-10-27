using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface ITenantSubscriptionRepository
    {
        Task<IEnumerable<TenantSubscription>> GetAllAsync();

        Task<TenantSubscription?> GetByIdAsync(
            long subscriptionId);

        Task<long> CreateAsync(
            TenantSubscription subscription);

        Task<bool> UpdateAsync(
            TenantSubscription subscription);

        Task<bool> DeleteAsync(
            long subscriptionId);
    }
}
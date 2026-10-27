namespace SMS.Application
{
    public interface ITenantSubscriptionService
    {
        Task<IEnumerable<TenantSubscriptionDto>> GetAllAsync();

        Task<TenantSubscriptionDto?> GetByIdAsync(
            long subscriptionId);

        Task<long> CreateAsync(
            TenantSubscriptionDto subscription);

        Task<bool> UpdateAsync(
            TenantSubscriptionDto subscription);

        Task<bool> DeleteAsync(
            long subscriptionId);
    }
}

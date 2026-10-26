namespace SMS.Application
{
    public interface ITenantSubscriptionService
    {
        Task<IEnumerable<TenantSubscriptionDto>> GetAllAsync();
        Task<TenantSubscriptionDto?> GetByIdAsync(long subscriptionId);
        Task<long> CreateAsync(TenantSubscriptionDto dto);
        Task<bool> UpdateAsync(TenantSubscriptionDto dto);
        Task<bool> DeleteAsync(long subscriptionId);
    }
}
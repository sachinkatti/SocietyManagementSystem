namespace SMS.Application
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantDto>> GetAllAsync();
        Task<TenantDto?> GetByIdAsync(long tenantId);
        Task<long> CreateAsync(TenantDto dto);
        Task<bool> UpdateAsync(TenantDto dto);
        Task<bool> DeleteAsync(long tenantId);
    }
}

using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IMaintenanceBillRepository
    {
        Task<List<MaintenanceBill>> GetAllAsync();

        Task<MaintenanceBill?> GetByIdAsync(long billId);

        Task<List<MaintenanceBill>> GetByTenantAsync(long tenantId);

        Task<List<MaintenanceBill>> GetFlatsByTenantAsync(long tenantId);

        Task<long> CreateAsync(MaintenanceBill bill);

        Task<bool> UpdateAsync(MaintenanceBill bill);

        Task<bool> DeleteAsync(
            long billId,
            long? modifyBy,
            long tenantId);
    }
}

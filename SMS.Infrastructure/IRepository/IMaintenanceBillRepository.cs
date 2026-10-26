using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IMaintenanceBillRepository
    {
        Task<List<MaintenanceBill>> GetAllAsync();
        Task<MaintenanceBill?> GetByIdAsync(long id);
        Task<long> CreateAsync(MaintenanceBill bill);
        Task<bool> UpdateAsync(MaintenanceBill bill);
        Task<bool> DeleteAsync(long id);
    }
}
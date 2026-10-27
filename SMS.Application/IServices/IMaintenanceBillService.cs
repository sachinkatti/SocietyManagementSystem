namespace SMS.Application
{
    public interface IMaintenanceBillService
    {
        Task<List<MaintenanceBillDto>> GetAllAsync();

        Task<MaintenanceBillDto?> GetByIdAsync(long billId);

        Task<List<MaintenanceBillDto>> GetByTenantAsync(long tenantId);

        Task<long> CreateAsync(
            MaintenanceBillDto dto,
            long? createdBy);

        Task<bool> UpdateAsync(
            MaintenanceBillDto dto,
            long? modifyBy);

        Task<bool> DeleteAsync(
            long billId,
            long? modifyBy,
            long tenantId);

        Task<List<MaintenanceBillDto>> GetFlatsByTenantAsync(long tenantId);
    }
}

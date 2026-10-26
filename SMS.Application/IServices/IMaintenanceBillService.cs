namespace SMS.Application
{
    public interface IMaintenanceBillService
    {
        Task<List<MaintenanceBillDto>> GetAllAsync();
        Task<MaintenanceBillDto?> GetByIdAsync(long id);
        Task<long> CreateAsync(MaintenanceBillDto dto);
        Task<bool> UpdateAsync(MaintenanceBillDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
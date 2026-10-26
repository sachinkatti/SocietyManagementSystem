namespace SMS.Application
{
    public interface IVehicleRegistryService
    {
        Task<IEnumerable<VehicleRegistryDto>> GetAllAsync();
        Task<VehicleRegistryDto?> GetByIdAsync(long id);
        Task<long> CreateAsync(VehicleRegistryDto dto);
        Task<bool> UpdateAsync(VehicleRegistryDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
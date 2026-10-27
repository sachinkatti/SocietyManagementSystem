using SMS.Infrastructure;

namespace SMS.Application
{
    public interface IVehicleRegistryService
    {
        Task<IEnumerable<VehicleRegistryDto>> GetAllAsync();

        Task<VehicleRegistryDto?> GetByIdAsync(long vehicleId);

        Task<long> CreateAsync(VehicleRegistryDto dto);

        Task<bool> UpdateAsync(VehicleRegistryDto dto);

        Task<bool> DeleteAsync(long vehicleId);

        Task<IEnumerable<VehicleFlatOption>> GetFlatsByTenantAsync(long tenantId);
    }
}

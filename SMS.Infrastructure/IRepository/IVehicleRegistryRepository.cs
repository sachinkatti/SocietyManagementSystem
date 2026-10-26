using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IVehicleRegistryRepository
    {
        Task<IEnumerable<VehicleRegistry>> GetAllAsync();
        Task<VehicleRegistry?> GetByIdAsync(long id);
        Task<long> CreateAsync(VehicleRegistry vehicle);
        Task<bool> UpdateAsync(VehicleRegistry vehicle);
        Task<bool> DeleteAsync(long id);
    }
}
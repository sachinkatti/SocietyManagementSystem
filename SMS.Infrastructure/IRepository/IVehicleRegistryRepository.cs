
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IVehicleRegistryRepository
    {
        Task<IEnumerable<VehicleRegistry>> GetAllAsync();

        Task<VehicleRegistry?> GetByIdAsync(long vehicleId);

        Task<long> CreateAsync(VehicleRegistry vehicle);

        Task<bool> UpdateAsync(VehicleRegistry vehicle);

        Task<bool> DeleteAsync(long vehicleId, long? modifyBy = null);

        Task<IEnumerable<VehicleFlatOption>> GetFlatsByTenantAsync(long tenantId);
    }

    public class VehicleFlatOption
    {
        public long FlatId { get; set; }
        public string FlatNumber { get; set; } = string.Empty;
        public long WingId { get; set; }
        public string WingName { get; set; } = string.Empty;
    }
}
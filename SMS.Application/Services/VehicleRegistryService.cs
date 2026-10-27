using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class VehicleRegistryService : IVehicleRegistryService
    {
        private readonly IVehicleRegistryRepository _repository;

        public VehicleRegistryService(IVehicleRegistryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VehicleRegistryDto>> GetAllAsync()
        {
            var vehicles = await _repository.GetAllAsync();
            return vehicles.Select(MapToDto).ToList();
        }

        public async Task<VehicleRegistryDto?> GetByIdAsync(long vehicleId)
        {
            var vehicle = await _repository.GetByIdAsync(vehicleId);

            return vehicle == null ? null : MapToDto(vehicle);
        }

        public async Task<long> CreateAsync(VehicleRegistryDto dto)
        {
            var vehicle = MapToEntity(dto);
            return await _repository.CreateAsync(vehicle);
        }

        public async Task<bool> UpdateAsync(VehicleRegistryDto dto)
        {
            var vehicle = MapToEntity(dto);
            return await _repository.UpdateAsync(vehicle);
        }

        public async Task<bool> DeleteAsync(long vehicleId)
        {
            return await _repository.DeleteAsync(vehicleId);
        }

        public async Task<IEnumerable<VehicleFlatOption>> GetFlatsByTenantAsync(
            long tenantId)
        {
            return await _repository.GetFlatsByTenantAsync(tenantId);
        }

        private static VehicleRegistryDto MapToDto(VehicleRegistry vehicle)
        {
            return new VehicleRegistryDto
            {
                VehicleId = vehicle.VehicleId,
                TenantId = vehicle.TenantId,
                TenantName = vehicle.TenantName,
                FlatId = vehicle.FlatId,
                FlatNumber = vehicle.FlatNumber,
                WingName = vehicle.WingName,
                VehicleNumber = vehicle.VehicleNumber,
                VehicleType = vehicle.VehicleType,
                Description = vehicle.Description,
                IsActive = vehicle.IsActive,
                CreatedOn = vehicle.CreatedOn,
                CreatedBy = vehicle.CreatedBy,
                ModifyOn = vehicle.ModifyOn,
                ModifyBy = vehicle.ModifyBy
            };
        }

        private static VehicleRegistry MapToEntity(VehicleRegistryDto dto)
        {
            return new VehicleRegistry
            {
                VehicleId = dto.VehicleId,
                TenantId = dto.TenantId,
                FlatId = dto.FlatId,
                VehicleNumber = dto.VehicleNumber.Trim().ToUpperInvariant(),
                VehicleType = dto.VehicleType.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim(),
                IsActive = true,
                CreatedBy = dto.CreatedBy,
                ModifyBy = dto.ModifyBy
            };
        }
    }
}
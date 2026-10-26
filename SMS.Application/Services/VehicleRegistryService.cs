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
            return (await _repository.GetAllAsync()).Select(ToDto);
        }

        public async Task<VehicleRegistryDto?> GetByIdAsync(long id)
        {
            var v = await _repository.GetByIdAsync(id);
            return v == null ? null : ToDto(v);
        }

        public Task<long> CreateAsync(VehicleRegistryDto dto)
        {
            return _repository.CreateAsync(ToEntity(dto));
        }

        public Task<bool> UpdateAsync(VehicleRegistryDto dto)
        {
            return _repository.UpdateAsync(ToEntity(dto));
        }

        public Task<bool> DeleteAsync(long id)
        {
            return _repository.DeleteAsync(id);
        }

        private static VehicleRegistryDto ToDto(VehicleRegistry v) => new()
        {
            VehicleId = v.VehicleId,
            FlatId = v.FlatId,
            TenantId = v.TenantId,
            VehicleNumber = v.VehicleNumber,
            VehicleType = v.VehicleType,
            Description = v.Description,
            IsActive = v.IsActive,
            CreatedOn = v.CreatedOn,
            CreatedBy = v.CreatedBy,
            ModifyOn = v.ModifyOn,
            ModifyBy = v.ModifyBy
        };

        private static VehicleRegistry ToEntity(VehicleRegistryDto d) => new()
        {
            VehicleId = d.VehicleId,
            FlatId = d.FlatId,
            TenantId = d.TenantId,
            VehicleNumber = d.VehicleNumber,
            VehicleType = d.VehicleType,
            Description = d.Description,
            IsActive = d.IsActive,
            CreatedBy = d.CreatedBy,
            ModifyBy = d.ModifyBy
        };
    }
}
using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            var permissions = await _permissionRepository.GetAllAsync();

            return permissions.Select(MapToDto);
        }

        public async Task<PermissionDto?> GetByIdAsync(long permissionId)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);

            return permission == null ? null : MapToDto(permission);
        }

        public async Task<long> CreateAsync(PermissionDto dto)
        {
            var permission = MapToEntity(dto);

            return await _permissionRepository.CreateAsync(permission);
        }

        public async Task<bool> UpdateAsync(PermissionDto dto)
        {
            var permission = MapToEntity(dto);

            return await _permissionRepository.UpdateAsync(permission);
        }

        public async Task<bool> DeleteAsync(long permissionId, long? modifyBy)
        {
            return await _permissionRepository.DeleteAsync(permissionId, modifyBy);
        }

        private static PermissionDto MapToDto(Permission permission)
        {
            return new PermissionDto
            {
                PermissionId = permission.PermissionId,
                PermissionName = permission.PermissionName,
                PermissionCode = permission.PermissionCode,
                Description = permission.Description,
                IsActive = permission.IsActive,
                CreatedOn = permission.CreatedOn,
                CreatedBy = permission.CreatedBy,
                ModifyOn = permission.ModifyOn,
                ModifyBy = permission.ModifyBy
            };
        }

        private static Permission MapToEntity(PermissionDto dto)
        {
            return new Permission
            {
                PermissionId = dto.PermissionId,
                PermissionName = dto.PermissionName,
                PermissionCode = dto.PermissionCode,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy,
                ModifyBy = dto.ModifyBy
            };
        }
    }
}

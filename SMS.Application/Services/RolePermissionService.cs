using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RolePermissionService(
            IRolePermissionRepository rolePermissionRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<IEnumerable<RolePermissionDto>> GetAllAsync()
        {
            var mappings = await _rolePermissionRepository.GetAllAsync();

            return mappings.Select(MapToDto);
        }

        public async Task<RolePermissionDto?> GetByIdAsync(long rolePermissionId)
        {
            var mapping = await _rolePermissionRepository.GetByIdAsync(
                rolePermissionId);

            return mapping == null ? null : MapToDto(mapping);
        }

        public async Task<IEnumerable<RolePermissionDto>> GetByRoleIdAsync(
            long roleId)
        {
            var mappings = await _rolePermissionRepository.GetByRoleIdAsync(roleId);

            return mappings.Select(MapToDto);
        }

        public async Task<long> AssignAsync(RolePermissionDto dto)
        {
            var mapping = MapToEntity(dto);

            return await _rolePermissionRepository.AssignAsync(mapping);
        }

        public async Task<bool> RemoveAsync(
            long rolePermissionId,
            long? modifyBy)
        {
            return await _rolePermissionRepository.RemoveAsync(
                rolePermissionId,
                modifyBy);
        }

        private static RolePermissionDto MapToDto(RolePermission mapping)
        {
            return new RolePermissionDto
            {
                RolePermissionId = mapping.RolePermissionId,
                RoleId = mapping.RoleId,
                RoleName = mapping.RoleName,
                PermissionId = mapping.PermissionId,
                PermissionName = mapping.PermissionName,
                PermissionCode = mapping.PermissionCode,
                Description = mapping.Description,
                IsActive = mapping.IsActive,
                CreatedOn = mapping.CreatedOn,
                CreatedBy = mapping.CreatedBy,
                ModifyOn = mapping.ModifyOn,
                ModifyBy = mapping.ModifyBy
            };
        }

        private static RolePermission MapToEntity(RolePermissionDto dto)
        {
            return new RolePermission
            {
                RolePermissionId = dto.RolePermissionId,
                RoleId = dto.RoleId,
                PermissionId = dto.PermissionId,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy,
                ModifyBy = dto.ModifyBy
            };
        }
    }
}

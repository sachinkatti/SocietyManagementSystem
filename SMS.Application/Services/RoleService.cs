using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();

            return roles.Select(MapToDto);
        }

        public async Task<RoleDto?> GetByIdAsync(long roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);

            return role == null ? null : MapToDto(role);
        }

        public async Task<long> CreateAsync(RoleDto dto)
        {
            var role = MapToEntity(dto);

            return await _roleRepository.CreateAsync(role);
        }

        public async Task<bool> UpdateAsync(RoleDto dto)
        {
            var role = MapToEntity(dto);

            return await _roleRepository.UpdateAsync(role);
        }

        public async Task<bool> DeleteAsync(long roleId, long? modifyBy)
        {
            return await _roleRepository.DeleteAsync(roleId, modifyBy);
        }

        private static RoleDto MapToDto(Role role)
        {
            return new RoleDto
            {
                RoleId = role.RoleId,
                TenantId = role.TenantId,
                RoleName = role.RoleName,
                RoleCode = role.RoleCode,
                RoleDescription = role.RoleDescription,
                IsActive = role.IsActive,
                CreatedOn = role.CreatedOn,
                CreatedBy = role.CreatedBy,
                ModifyOn = role.ModifyOn,
                ModifyBy = role.ModifyBy
            };
        }

        private static Role MapToEntity(RoleDto dto)
        {
            return new Role
            {
                RoleId = dto.RoleId,
                TenantId = dto.TenantId,
                RoleName = dto.RoleName,
                RoleCode = dto.RoleCode,
                RoleDescription = dto.RoleDescription,
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy,
                ModifyBy = dto.ModifyBy
            };
        }
    }
}

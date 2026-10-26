using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleService(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public async Task<IEnumerable<UserRoleDto>> GetAllAsync()
        {
            var userRoles = await _userRoleRepository.GetAllAsync();

            return userRoles.Select(MapToDto);
        }

        public async Task<UserRoleDto?> GetByIdAsync(long userRoleId)
        {
            var userRole = await _userRoleRepository.GetByIdAsync(userRoleId);

            return userRole == null ? null : MapToDto(userRole);
        }

        public async Task<IEnumerable<UserRoleDto>> GetByUserIdAsync(long userId)
        {
            var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);

            return userRoles.Select(MapToDto);
        }

        public async Task<long> AssignAsync(UserRoleDto dto)
        {
            var userRole = MapToEntity(dto);

            return await _userRoleRepository.AssignAsync(userRole);
        }

        public async Task<bool> RemoveAsync(long userRoleId, long? modifyBy)
        {
            return await _userRoleRepository.RemoveAsync(userRoleId, modifyBy);
        }

        private static UserRoleDto MapToDto(UserRole userRole)
        {
            return new UserRoleDto
            {
                UserRoleId = userRole.UserRoleId,
                UserId = userRole.UserId,
                UserName = userRole.UserName,
                RoleId = userRole.RoleId,
                RoleName = userRole.RoleName,
                RoleCode = userRole.RoleCode,
                Description = userRole.Description,
                IsActive = userRole.IsActive,
                CreatedOn = userRole.CreatedOn,
                CreatedBy = userRole.CreatedBy,
                ModifyOn = userRole.ModifyOn,
                ModifyBy = userRole.ModifyBy
            };
        }

        private static UserRole MapToEntity(UserRoleDto dto)
        {
            return new UserRole
            {
                UserRoleId = dto.UserRoleId,
                UserId = dto.UserId,
                RoleId = dto.RoleId,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy,
                ModifyBy = dto.ModifyBy
            };
        }
    }
}

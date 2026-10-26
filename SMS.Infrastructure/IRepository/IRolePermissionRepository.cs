
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IRolePermissionRepository
    {
        Task<IEnumerable<RolePermission>> GetAllAsync();
        Task<RolePermission?> GetByIdAsync(long rolePermissionId);
        Task<IEnumerable<RolePermission>> GetByRoleIdAsync(long roleId);
        Task<long> AssignAsync(RolePermission rolePermission);
        Task<bool> RemoveAsync(long rolePermissionId, long? modifyBy);
    }
}

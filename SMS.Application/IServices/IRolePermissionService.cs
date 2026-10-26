namespace SMS.Application
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<RolePermissionDto>> GetAllAsync();
        Task<RolePermissionDto?> GetByIdAsync(long rolePermissionId);
        Task<IEnumerable<RolePermissionDto>> GetByRoleIdAsync(long roleId);
        Task<long> AssignAsync(RolePermissionDto dto);
        Task<bool> RemoveAsync(long rolePermissionId, long? modifyBy);
    }
}

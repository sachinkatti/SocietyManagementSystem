namespace SMS.Application
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<RoleDto?> GetByIdAsync(long roleId);
        Task<long> CreateAsync(RoleDto dto);
        Task<bool> UpdateAsync(RoleDto dto);
        Task<bool> DeleteAsync(long roleId, long? modifyBy);
    }
}

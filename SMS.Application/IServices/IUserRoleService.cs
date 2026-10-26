namespace SMS.Application
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRoleDto>> GetAllAsync();
        Task<UserRoleDto?> GetByIdAsync(long userRoleId);
        Task<IEnumerable<UserRoleDto>> GetByUserIdAsync(long userId);
        Task<long> AssignAsync(UserRoleDto dto);
        Task<bool> RemoveAsync(long userRoleId, long? modifyBy);
    }
}

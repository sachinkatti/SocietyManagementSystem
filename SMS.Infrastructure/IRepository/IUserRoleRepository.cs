
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IUserRoleRepository
    {
        Task<IEnumerable<UserRole>> GetAllAsync();
        Task<UserRole?> GetByIdAsync(long userRoleId);
        Task<IEnumerable<UserRole>> GetByUserIdAsync(long userId);
        Task<long> AssignAsync(UserRole userRole);
        Task<bool> RemoveAsync(long userRoleId, long? modifyBy);
    }
}

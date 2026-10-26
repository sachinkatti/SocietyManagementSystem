using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(long roleId);
        Task<long> CreateAsync(Role role);
        Task<bool> UpdateAsync(Role role);
        Task<bool> DeleteAsync(long roleId, long? modifyBy);
    }
}
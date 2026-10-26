
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<Permission?> GetByIdAsync(long permissionId);
        Task<long> CreateAsync(Permission permission);
        Task<bool> UpdateAsync(Permission permission);
        Task<bool> DeleteAsync(long permissionId, long? modifyBy);
    }
}

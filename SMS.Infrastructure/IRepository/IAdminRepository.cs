
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IAdminRepository
    {
        Task<List<Admin>> GetAllAsync();

        Task<Admin?> GetByIdAsync(long adminId);

        Task<long> CreateAsync(Admin admin, long? createdBy);

        Task<bool> UpdateAsync(Admin admin, long? modifyBy);

        Task<bool> DeleteAsync(long adminId, long? modifyBy);

        Task<List<User>> GetAvailableUsersForAdminAsync();
    }
}
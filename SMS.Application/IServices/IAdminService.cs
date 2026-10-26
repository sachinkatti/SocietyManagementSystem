
using SMS.Application.DTOs;

namespace SMS.Application
{
    public interface IAdminService
    {
        Task<List<AdminDto>> GetAllAsync();

        Task<AdminDto?> GetByIdAsync(long adminId);

        Task<long> CreateAsync(AdminDto admin, long? createdBy);

        Task<bool> UpdateAsync(AdminDto admin, long? modifyBy);

        Task<bool> DeleteAsync(long adminId, long? modifyBy);

        Task<List<UserDto>> GetAvailableUsersForAdminAsync();
    }
}

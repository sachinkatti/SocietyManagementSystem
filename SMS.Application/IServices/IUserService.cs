namespace SMS.Application
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(long userId);
        Task<long> CreateAsync(UserDto dto);
        Task<bool> UpdateAsync(UserDto dto);
        Task<bool> DeleteAsync(long userId);
        Task<IEnumerable<UserDto>> GetActiveUsersForDropdownAsync();
    }
}
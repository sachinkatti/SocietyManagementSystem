using SMS.Domain;


namespace SMS.Infrastructure
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(long userId);
        Task<long> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(long userId);

        Task<IEnumerable<User>> GetActiveUsersForDropdownAsync();
    }
}
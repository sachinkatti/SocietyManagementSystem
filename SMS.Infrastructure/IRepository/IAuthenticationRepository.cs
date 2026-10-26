
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IAuthenticationRepository
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<List<string>> GetUserRolesAsync(long userId);

        Task<List<string>> GetUserPermissionsAsync(long userId);

        Task<bool> UpdateFailedLoginAsync(
            long userId,
            int failedLoginCount,
            DateTime? lockoutEndUtc);

        Task<bool> SetMissingPasswordHashAsync(
            long userId,
            string passwordHash);

        Task<bool> ChangePasswordAsync(
            long userId,
            string passwordHash);

        Task UpdateSuccessfulLoginAsync(long userId);
    }
}

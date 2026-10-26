using SMS.Domain;

namespace SMS.Application
{
    public interface IPasswordHasherService
    {
        string HashPassword(User user, string password);

        bool VerifyPassword(
            User user,
            string passwordHash,
            string providedPassword);

        bool VerifyExistingHash(User user, string password);
    }
}
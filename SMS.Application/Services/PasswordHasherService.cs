using Microsoft.AspNetCore.Identity;
using SMS.Domain;

namespace SMS.Application
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<User> _passwordHasher = new();

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(
            User user,
            string passwordHash,
            string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(passwordHash) ||
                string.IsNullOrWhiteSpace(providedPassword))
            {
                return false;
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user, passwordHash, providedPassword);

            return result != PasswordVerificationResult.Failed;
        }

        public bool VerifyExistingHash(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
                return false;

            return _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password
            ) != PasswordVerificationResult.Failed;
        }
    }
}
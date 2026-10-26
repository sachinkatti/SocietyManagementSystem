using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IPasswordHasherService _passwordHasherService;

        public AuthenticationService(
            IAuthenticationRepository authenticationRepository,
            IPasswordHasherService passwordHasherService)
        {
            _authenticationRepository = authenticationRepository;
            _passwordHasherService = passwordHasherService;
        }


        public async Task<LoginResultDto> LoginAsync(LoginDto loginDto)
        {
            Console.WriteLine("LOGIN DEBUG: LoginAsync reached.");
            Console.WriteLine($"LOGIN DEBUG: Email supplied: {!string.IsNullOrWhiteSpace(loginDto.Email)}");
            Console.WriteLine($"LOGIN DEBUG: Password supplied: {!string.IsNullOrWhiteSpace(loginDto.Password)}");

            var result = new LoginResultDto
            {
                Success = false,
                Message = "Invalid email or password."
            };

            if (string.IsNullOrWhiteSpace(loginDto.Email) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                Console.WriteLine("LOGIN DEBUG: Email or password is empty.");
                return result;
            }

            var email = loginDto.Email.Trim();

            var user = await _authenticationRepository
                .GetUserByEmailAsync(email);

            Console.WriteLine($"LOGIN DEBUG: User found: {user != null}");

            if (user == null || !user.IsActive)
            {
                Console.WriteLine("LOGIN DEBUG: User missing or inactive.");
                return result;
            }

            if (user.LockoutEndUtc.HasValue &&
                user.LockoutEndUtc.Value > DateTime.UtcNow)
            {
                Console.WriteLine("LOGIN DEBUG: Account is locked.");
                result.Message = "Your account is temporarily locked. Please try again later.";
                return result;
            }

            var passwordValid = _passwordHasherService.VerifyPassword(
                user,
                user.PasswordHash ?? string.Empty,
                loginDto.Password);

            Console.WriteLine($"LOGIN DEBUG: Password valid: {passwordValid}");

            if (!passwordValid)
            {
                var failedCount = user.FailedLoginCount + 1;

                DateTime? lockoutEndUtc = failedCount >= 5
                    ? DateTime.UtcNow.AddMinutes(15)
                    : null;

                await _authenticationRepository.UpdateFailedLoginAsync(
                    user.UserId,
                    lockoutEndUtc.HasValue ? 0 : failedCount,
                    lockoutEndUtc);

                Console.WriteLine("LOGIN DEBUG: Failed-login update attempted.");

                if (lockoutEndUtc.HasValue)
                {
                    result.Message = "Too many failed attempts. Your account is locked for 15 minutes.";
                }

                return result;
            }

            Console.WriteLine("LOGIN DEBUG: Password verified.");

            await _authenticationRepository
                .UpdateSuccessfulLoginAsync(user.UserId);

            var roles = await _authenticationRepository
                .GetUserRolesAsync(user.UserId);

            var permissions = await _authenticationRepository
                .GetUserPermissionsAsync(user.UserId);

            result.Success = true;
            result.Message = "Login successful.";
            result.UserId = user.UserId;
            result.TenantId = user.TenantId;
            result.FullName = string.Join(
                " ",
                new[] { user.FirstName, user.MiddleName, user.LastName }
                    .Where(name => !string.IsNullOrWhiteSpace(name)));
            result.Email = user.Email;
            result.MustChangePassword = user.MustChangePassword;
            result.Roles = roles;
            result.Permissions = permissions;

            Console.WriteLine("LOGIN DEBUG: Login successful.");

            return result;
        }

    }
}
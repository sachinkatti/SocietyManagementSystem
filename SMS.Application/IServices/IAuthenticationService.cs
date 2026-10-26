namespace SMS.Application
{
    public interface IAuthenticationService
    {
        Task<LoginResultDto> LoginAsync(LoginDto loginDto);
    }
}
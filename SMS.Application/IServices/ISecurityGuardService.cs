namespace SMS.Application
{
    public interface ISecurityGuardService
    {
        Task<IEnumerable<SecurityGuardDto>> GetAllAsync();
        Task<SecurityGuardDto?> GetByIdAsync(long guardId);
        Task<long> CreateAsync(SecurityGuardDto dto);
        Task<bool> UpdateAsync(SecurityGuardDto dto);
        Task<bool> DeleteAsync(long guardId);
    }
}

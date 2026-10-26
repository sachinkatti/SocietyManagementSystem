namespace SMS.Application
{
    public interface IWingService
    {
        Task<IEnumerable<WingDto>> GetAllAsync();
        Task<WingDto?> GetByIdAsync(long wingId);
        Task<long> CreateAsync(WingDto dto);
        Task<bool> UpdateAsync(WingDto dto);
        Task<bool> DeleteAsync(long wingId);
    }
}

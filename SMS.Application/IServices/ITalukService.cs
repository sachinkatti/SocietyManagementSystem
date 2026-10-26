namespace SMS.Application
{
    public interface ITalukService
    {
        Task<IEnumerable<TalukDto>> GetAllAsync();
        Task<TalukDto?> GetByIdAsync(long talukId);
        Task<long> CreateAsync(TalukDto dto);
        Task<bool> UpdateAsync(TalukDto dto);
        Task<bool> DeleteAsync(long talukId);
    }
}
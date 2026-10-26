namespace SMS.Application
{
    public interface IVisitorLogService
    {
        Task<List<VisitorLogDto>> GetAllAsync();
        Task<VisitorLogDto?> GetByIdAsync(long visitorId);
        Task<long> CreateAsync(VisitorLogDto dto);
        Task<bool> UpdateAsync(VisitorLogDto dto);
        Task<bool> DeleteAsync(long visitorId, long? modifyBy);
    }
}
namespace SMS.Application
{
    public interface IResidentService
    {
        Task<IEnumerable<ResidentDto>> GetAllAsync();
        Task<ResidentDto?> GetByIdAsync(long residentId);
        Task<long> CreateAsync(ResidentDto dto);
        Task<bool> UpdateAsync(ResidentDto dto);
        Task<bool> DeleteAsync(long residentId);
    }
}

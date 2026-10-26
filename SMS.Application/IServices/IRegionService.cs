namespace SMS.Application
{
    public interface IRegionService
    {
        Task<IEnumerable<RegionDto>> GetAllAsync();
        Task<RegionDto?> GetByIdAsync(long regionId);
        Task<long> CreateAsync(RegionDto dto);
        Task<bool> UpdateAsync(RegionDto dto);
        Task<bool> DeleteAsync(long regionId);
    }
}
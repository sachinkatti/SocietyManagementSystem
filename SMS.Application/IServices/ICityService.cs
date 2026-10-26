namespace SMS.Application
{
    public interface ICityService
    {
        Task<IEnumerable<CityDto>> GetAllAsync();
        Task<CityDto?> GetByIdAsync(long cityId);
        Task<long> CreateAsync(CityDto dto);
        Task<bool> UpdateAsync(CityDto dto);
        Task<bool> DeleteAsync(long cityId);
    }
}
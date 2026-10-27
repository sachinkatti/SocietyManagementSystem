namespace SMS.Application
{
    public interface IAmenityService
    {
        Task<IEnumerable<AmenityDto>> GetAllAsync();

        Task<AmenityDto?> GetByIdAsync(long amenityId);

        Task<long> CreateAsync(AmenityDto dto);

        Task<bool> UpdateAsync(AmenityDto dto);

        Task<bool> DeleteAsync(long amenityId);

    }
}

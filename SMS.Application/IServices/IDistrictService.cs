namespace SMS.Application
{
    public interface IDistrictService
    {
        Task<IEnumerable<DistrictDto>> GetAllAsync();
        Task<DistrictDto?> GetByIdAsync(long districtId);
        Task<long> CreateAsync(DistrictDto dto);
        Task<bool> UpdateAsync(DistrictDto dto);
        Task<bool> DeleteAsync(long districtId);
    }
}
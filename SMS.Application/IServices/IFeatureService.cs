namespace SMS.Application
{
    public interface IFeatureService
    {
        Task<IEnumerable<FeatureDto>> GetAllAsync();
        Task<FeatureDto?> GetByIdAsync(long featureId);
        Task<long> CreateAsync(FeatureDto dto);
        Task<bool> UpdateAsync(FeatureDto dto);
        Task<bool> DeleteAsync(long featureId, long? modifyBy);
    }
}
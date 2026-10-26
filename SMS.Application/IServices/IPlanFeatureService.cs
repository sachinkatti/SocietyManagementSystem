namespace SMS.Application
{
    public interface IPlanFeatureService
    {
        Task<IEnumerable<PlanFeatureDto>> GetAllAsync();
        Task<PlanFeatureDto?> GetByIdAsync(long planFeatureId);
        Task<long> CreateAsync(PlanFeatureDto dto);
        Task<bool> UpdateAsync(PlanFeatureDto dto);
        Task<bool> DeleteAsync(long planFeatureId, long? modifyBy);
    }
}

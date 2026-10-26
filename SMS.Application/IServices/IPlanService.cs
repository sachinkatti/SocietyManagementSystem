namespace SMS.Application
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanDto>> GetAllAsync();
        Task<PlanDto?> GetByIdAsync(long planId);
        Task<long> CreateAsync(PlanDto dto);
        Task<bool> UpdateAsync(PlanDto dto);
        Task<bool> DeleteAsync(long planId, long? modifyBy);
    }
}

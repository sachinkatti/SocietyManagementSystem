using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;

        public PlanService(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<IEnumerable<PlanDto>> GetAllAsync()
        {
            var plans = await _planRepository.GetAllAsync();

            return plans.Select(MapToDto);
        }

        public async Task<PlanDto?> GetByIdAsync(long planId)
        {
            var plan = await _planRepository.GetByIdAsync(planId);

            return plan == null ? null : MapToDto(plan);
        }

        public async Task<long> CreateAsync(PlanDto dto)
        {
            var plan = new Plan
            {
                PlanName = dto.PlanName.Trim(),
                PlanCode = dto.PlanCode.Trim(),
                BillingCycle = dto.BillingCycle.Trim(),
                Price = dto.Price,
                Description = dto.Description?.Trim(),
                IsActive = true,
                CreatedBy = dto.CreatedBy
            };

            return await _planRepository.CreateAsync(plan);
        }

        public async Task<bool> UpdateAsync(PlanDto dto)
        {
            var plan = new Plan
            {
                PlanId = dto.PlanId,
                PlanName = dto.PlanName.Trim(),
                PlanCode = dto.PlanCode.Trim(),
                BillingCycle = dto.BillingCycle.Trim(),
                Price = dto.Price,
                Description = dto.Description?.Trim(),
                IsActive = dto.IsActive,
                ModifyBy = dto.ModifyBy
            };

            return await _planRepository.UpdateAsync(plan);
        }

        public Task<bool> DeleteAsync(long planId, long? modifyBy)
        {
            return _planRepository.DeleteAsync(planId, modifyBy);
        }

        private static PlanDto MapToDto(Plan plan)
        {
            return new PlanDto
            {
                PlanId = plan.PlanId,
                PlanName = plan.PlanName,
                PlanCode = plan.PlanCode,
                BillingCycle = plan.BillingCycle,
                Price = plan.Price,
                Description = plan.Description,
                IsActive = plan.IsActive,
                CreatedOn = plan.CreatedOn,
                CreatedBy = plan.CreatedBy,
                ModifyOn = plan.ModifyOn,
                ModifyBy = plan.ModifyBy
            };
        }
    }
}

using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class PlanFeatureService : IPlanFeatureService
    {
        private readonly IPlanFeatureRepository _repository;

        public PlanFeatureService(IPlanFeatureRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PlanFeatureDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<PlanFeatureDto?> GetByIdAsync(long planFeatureId)
        {
            var item = await _repository.GetByIdAsync(planFeatureId);
            return item == null ? null : MapToDto(item);
        }

        public async Task<long> CreateAsync(PlanFeatureDto dto)
        {
            var item = new PlanFeature
            {
                PlanId = dto.PlanId,
                FeatureId = dto.FeatureId,
                FeatureValue = dto.FeatureValue.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim(),
                CreatedBy = dto.CreatedBy
            };

            return await _repository.CreateAsync(item);
        }

        public async Task<bool> UpdateAsync(PlanFeatureDto dto)
        {
            var item = new PlanFeature
            {
                PlanFeatureId = dto.PlanFeatureId,
                PlanId = dto.PlanId,
                FeatureId = dto.FeatureId,
                FeatureValue = dto.FeatureValue.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim(),
                IsActive = dto.IsActive,
                ModifyBy = dto.ModifyBy
            };

            return await _repository.UpdateAsync(item);
        }

        public Task<bool> DeleteAsync(long planFeatureId, long? modifyBy)
        {
            return _repository.DeleteAsync(planFeatureId, modifyBy);
        }

        private static PlanFeatureDto MapToDto(PlanFeature item)
        {
            return new PlanFeatureDto
            {
                PlanFeatureId = item.PlanFeatureId,
                PlanId = item.PlanId,
                PlanName = item.PlanName,
                FeatureId = item.FeatureId,
                FeatureName = item.FeatureName,
                FeatureCode = item.FeatureCode,
                FeatureValue = item.FeatureValue,
                Description = item.Description,
                IsActive = item.IsActive,
                CreatedOn = item.CreatedOn,
                CreatedBy = item.CreatedBy,
                ModifyOn = item.ModifyOn,
                ModifyBy = item.ModifyBy
            };
        }
    }
}

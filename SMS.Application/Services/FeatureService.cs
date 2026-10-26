using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class FeatureService : IFeatureService
    {
        private readonly IFeatureRepository _featureRepository;

        public FeatureService(IFeatureRepository featureRepository)
        {
            _featureRepository = featureRepository;
        }

        public async Task<IEnumerable<FeatureDto>> GetAllAsync()
        {
            var features = await _featureRepository.GetAllAsync();

            return features.Select(MapToDto);
        }

        public async Task<FeatureDto?> GetByIdAsync(long featureId)
        {
            var feature = await _featureRepository.GetByIdAsync(featureId);

            return feature == null ? null : MapToDto(feature);
        }

        public async Task<long> CreateAsync(FeatureDto dto)
        {
            var feature = new Feature
            {
                FeatureName = dto.FeatureName.Trim(),
                FeatureCode = dto.FeatureCode.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = true,
                CreatedBy = dto.CreatedBy
            };

            return await _featureRepository.CreateAsync(feature);
        }

        public async Task<bool> UpdateAsync(FeatureDto dto)
        {
            var feature = new Feature
            {
                FeatureId = dto.FeatureId,
                FeatureName = dto.FeatureName.Trim(),
                FeatureCode = dto.FeatureCode.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = dto.IsActive,
                ModifyBy = dto.ModifyBy
            };

            return await _featureRepository.UpdateAsync(feature);
        }

        public Task<bool> DeleteAsync(long featureId, long? modifyBy)
        {
            return _featureRepository.DeleteAsync(featureId, modifyBy);
        }

        private static FeatureDto MapToDto(Feature feature)
        {
            return new FeatureDto
            {
                FeatureId = feature.FeatureId,
                FeatureName = feature.FeatureName,
                FeatureCode = feature.FeatureCode,
                Description = feature.Description,
                IsActive = feature.IsActive,
                CreatedOn = feature.CreatedOn,
                CreatedBy = feature.CreatedBy,
                ModifyOn = feature.ModifyOn,
                ModifyBy = feature.ModifyBy
            };
        }
    }
}
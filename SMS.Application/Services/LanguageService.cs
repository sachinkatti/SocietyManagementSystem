using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;

        public LanguageService(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }

        public async Task<IEnumerable<LanguageDto>> GetAllAsync()
        {
            var languages = await _languageRepository.GetAllAsync();

            return languages.Select(MapToDto);
        }

        public async Task<LanguageDto?> GetByIdAsync(long languageId)
        {
            var language = await _languageRepository.GetByIdAsync(languageId);

            return language == null ? null : MapToDto(language);
        }

        public async Task<long> CreateAsync(LanguageDto dto)
        {
            var language = new Language
            {
                LanguageCode = dto.LanguageCode.Trim(),
                LanguageName = dto.LanguageName.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = true,
                CreatedOn = DateTime.Now,
                CreatedBy = 1,
                ModifyOn = DateTime.Now,
                ModifyBy = 1
            };

            return await _languageRepository.CreateAsync(language);
        }

        public async Task<bool> UpdateAsync(LanguageDto dto)
        {
            var language = new Language
            {
                LanguageId = dto.LanguageId,
                LanguageCode = dto.LanguageCode.Trim(),
                LanguageName = dto.LanguageName.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = dto.IsActive,
                ModifyOn = DateTime.Now,
                ModifyBy = 1
            };

            return await _languageRepository.UpdateAsync(language);
        }

        public async Task<bool> DeleteAsync(long languageId)
        {
            return await _languageRepository.DeleteAsync(languageId);
        }

        private static LanguageDto MapToDto(Language language)
        {
            return new LanguageDto
            {
                LanguageId = language.LanguageId,
                LanguageCode = language.LanguageCode,
                LanguageName = language.LanguageName,
                Description = language.Description,
                IsActive = language.IsActive,
                CreatedOn = language.CreatedOn,
                CreatedBy = language.CreatedBy,
                ModifyOn = language.ModifyOn,
                ModifyBy = language.ModifyBy
            };
        }
    }
}
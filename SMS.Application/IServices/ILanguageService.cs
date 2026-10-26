namespace SMS.Application
{
    public interface ILanguageService
    {
        Task<IEnumerable<LanguageDto>> GetAllAsync();
        Task<LanguageDto?> GetByIdAsync(long languageId);
        Task<long> CreateAsync(LanguageDto dto);
        Task<bool> UpdateAsync(LanguageDto dto);
        Task<bool> DeleteAsync(long languageId);
    }
}
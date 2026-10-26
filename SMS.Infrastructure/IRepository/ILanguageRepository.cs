using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface ILanguageRepository
    {
        Task<IEnumerable<Language>> GetAllAsync();
        Task<Language?> GetByIdAsync(long languageId);
        Task<long> CreateAsync(Language language);
        Task<bool> UpdateAsync(Language language);
        Task<bool> DeleteAsync(long languageId);
    }
}
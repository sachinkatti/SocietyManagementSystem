using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IFlatRepository
    {
        Task<IEnumerable<Flat>> GetAllAsync();

        Task<Flat?> GetByIdAsync(long flatId);

        Task<long> CreateAsync(Flat flat);

        Task<bool> UpdateAsync(Flat flat);

        Task<bool> DeleteAsync(long flatId);
    }
}
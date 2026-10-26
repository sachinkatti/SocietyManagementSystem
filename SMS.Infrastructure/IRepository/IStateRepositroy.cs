using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IStateRepository
    {
        Task<IEnumerable<State>> GetAllAsync();
        Task<State?> GetByIdAsync(long stateId);
        Task<long> CreateAsync(State state);
        Task<bool> UpdateAsync(State state);
        Task<bool> DeleteAsync(long stateId);
    }
}
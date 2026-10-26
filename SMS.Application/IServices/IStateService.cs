namespace SMS.Application
{
    public interface IStateService
    {
        Task<IEnumerable<StateDto>> GetAllAsync();
        Task<StateDto?> GetByIdAsync(long stateId);
        Task<long> CreateAsync(StateDto dto);
        Task<bool> UpdateAsync(StateDto dto);
        Task<bool> DeleteAsync(long stateId);
    }
}
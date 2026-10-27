namespace SMS.Application
{
    public interface IFlatService
    {
        Task<IEnumerable<FlatDto>> GetAllAsync();

        Task<FlatDto?> GetByIdAsync(long flatId);

        Task<long> CreateAsync(FlatDto dto);

        Task<bool> UpdateAsync(FlatDto dto);

        Task<bool> DeleteAsync(long flatId);
    }
}
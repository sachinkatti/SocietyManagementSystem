namespace SMS.Application
{
    public interface IExpenseLedgerService
    {
        Task<IEnumerable<ExpenseLedgerDto>> GetAllAsync();
        Task<ExpenseLedgerDto?> GetByIdAsync(long ledgerId);
        Task<long> CreateAsync(ExpenseLedgerDto dto);
        Task<bool> UpdateAsync(ExpenseLedgerDto dto);
        Task<bool> DeleteAsync(long ledgerId);
    }
}
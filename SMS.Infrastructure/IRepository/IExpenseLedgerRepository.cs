using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IExpenseLedgerRepository
    {
        Task<IEnumerable<ExpenseLedger>> GetAllAsync();
        Task<ExpenseLedger?> GetByIdAsync(long ledgerId);
        Task<long> CreateAsync(ExpenseLedger ledger);
        Task<bool> UpdateAsync(ExpenseLedger ledger);
        Task<bool> DeleteAsync(long ledgerId);
    }
}
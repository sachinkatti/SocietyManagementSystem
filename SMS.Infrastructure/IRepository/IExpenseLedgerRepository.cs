
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IExpenseLedgerRepository
    {
        Task<List<ExpenseLedger>> GetAllAsync();

        Task<ExpenseLedger?> GetByIdAsync(long ledgerId);

        Task<List<ExpenseLedger>> GetByTenantAsync(long tenantId);

        Task<List<ExpenseLedger>> GetAdminsByTenantAsync(long tenantId);

        Task<long> CreateAsync(ExpenseLedger expenseLedger);

        Task<bool> UpdateAsync(ExpenseLedger expenseLedger);

        Task<bool> DeleteAsync(
            long ledgerId,
            long? modifyBy,
            long tenantId);
    }
}

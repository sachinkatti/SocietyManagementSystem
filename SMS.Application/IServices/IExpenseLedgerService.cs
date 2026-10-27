namespace SMS.Application
{
    public interface IExpenseLedgerService
    {
        Task<List<ExpenseLedgerDto>> GetAllAsync();

        Task<ExpenseLedgerDto?> GetByIdAsync(long ledgerId);

        Task<List<ExpenseLedgerDto>> GetByTenantAsync(long tenantId);

        Task<List<ExpenseLedgerDto>> GetAdminsByTenantAsync(long tenantId);

        Task<long> CreateAsync(
            ExpenseLedgerDto dto,
            long? createdBy);

        Task<bool> UpdateAsync(
            ExpenseLedgerDto dto,
            long? modifyBy);

        Task<bool> DeleteAsync(
            long ledgerId,
            long? modifyBy,
            long tenantId);
    }
}

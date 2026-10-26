using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class ExpenseLedgerService : IExpenseLedgerService
    {
        private readonly IExpenseLedgerRepository _repository;

        public ExpenseLedgerService(IExpenseLedgerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ExpenseLedgerDto>> GetAllAsync()
        {
            var ledgers = await _repository.GetAllAsync();

            return ledgers.Select(MapToDto);
        }

        public async Task<ExpenseLedgerDto?> GetByIdAsync(long ledgerId)
        {
            var ledger = await _repository.GetByIdAsync(ledgerId);

            return ledger == null ? null : MapToDto(ledger);
        }

        public async Task<long> CreateAsync(ExpenseLedgerDto dto)
        {
            var ledger = new ExpenseLedger
            {
                EntryType = dto.EntryType.Trim(),
                Category = dto.Category.Trim(),
                Amount = dto.Amount,
                Description = dto.Description?.Trim(),
                EntryDate = dto.EntryDate.Date,
                RecordedByAdminId = dto.RecordedByAdminId,
                CreatedBy = dto.CreatedBy,
                TenantId = dto.TenantId
            };

            return await _repository.CreateAsync(ledger);
        }

        public async Task<bool> UpdateAsync(ExpenseLedgerDto dto)
        {
            var ledger = new ExpenseLedger
            {
                LedgerId = dto.LedgerId,
                EntryType = dto.EntryType.Trim(),
                Category = dto.Category.Trim(),
                Amount = dto.Amount,
                Description = dto.Description?.Trim(),
                EntryDate = dto.EntryDate.Date,
                RecordedByAdminId = dto.RecordedByAdminId,
                ModifyBy = dto.ModifyBy,
                TenantId = dto.TenantId
            };

            return await _repository.UpdateAsync(ledger);
        }

        public Task<bool> DeleteAsync(long ledgerId)
        {
            return _repository.DeleteAsync(ledgerId);
        }

        private static ExpenseLedgerDto MapToDto(ExpenseLedger ledger)
        {
            return new ExpenseLedgerDto
            {
                LedgerId = ledger.LedgerId,
                EntryType = ledger.EntryType,
                Category = ledger.Category,
                Amount = ledger.Amount,
                Description = ledger.Description,
                EntryDate = ledger.EntryDate,
                RecordedByAdminId = ledger.RecordedByAdminId,
                IsActive = ledger.IsActive,
                CreatedOn = ledger.CreatedOn,
                CreatedBy = ledger.CreatedBy,
                ModifyOn = ledger.ModifyOn,
                ModifyBy = ledger.ModifyBy,
                TenantId = ledger.TenantId
            };
        }
    }
}
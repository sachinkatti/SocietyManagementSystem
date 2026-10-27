using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class ExpenseLedgerService : IExpenseLedgerService
    {
        private readonly IExpenseLedgerRepository _repository;

        public ExpenseLedgerService(
            IExpenseLedgerRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ExpenseLedgerDto>> GetAllAsync()
        {
            var entries = await _repository.GetAllAsync();

            return entries.Select(MapToDto).ToList();
        }

        public async Task<ExpenseLedgerDto?> GetByIdAsync(long ledgerId)
        {
            var entry = await _repository.GetByIdAsync(ledgerId);

            return entry == null ? null : MapToDto(entry);
        }

        public async Task<List<ExpenseLedgerDto>> GetByTenantAsync(
            long tenantId)
        {
            var entries = await _repository.GetByTenantAsync(tenantId);

            return entries.Select(MapToDto).ToList();
        }

        public async Task<List<ExpenseLedgerDto>> GetAdminsByTenantAsync(
            long tenantId)
        {
            var admins = await _repository.GetAdminsByTenantAsync(tenantId);

            return admins.Select(MapToDto).ToList();
        }

        public async Task<long> CreateAsync(
            ExpenseLedgerDto dto,
            long? createdBy)
        {
            var entry = MapToEntity(dto);

            entry.CreatedBy = createdBy;

            return await _repository.CreateAsync(entry);
        }

        public async Task<bool> UpdateAsync(
            ExpenseLedgerDto dto,
            long? modifyBy)
        {
            var entry = MapToEntity(dto);

            entry.ModifyBy = modifyBy;

            return await _repository.UpdateAsync(entry);
        }

        public async Task<bool> DeleteAsync(
            long ledgerId,
            long? modifyBy,
            long tenantId)
        {
            return await _repository.DeleteAsync(
                ledgerId, modifyBy, tenantId);
        }

        private static ExpenseLedgerDto MapToDto(
            ExpenseLedger entry)
        {
            return new ExpenseLedgerDto
            {
                LedgerId = entry.LedgerId,
                EntryType = entry.EntryType,
                Category = entry.Category,
                Amount = entry.Amount,
                Description = entry.Description,
                EntryDate = entry.EntryDate,
                RecordedByAdminId = entry.RecordedByAdminId,
                RecordedByAdminName = entry.RecordedByAdminName,
                Designation = entry.Designation,
                IsActive = entry.IsActive,
                CreatedOn = entry.CreatedOn,
                CreatedBy = entry.CreatedBy,
                ModifyOn = entry.ModifyOn,
                ModifyBy = entry.ModifyBy,
                TenantId = entry.TenantId,
                TenantName = entry.TenantName
            };
        }

        private static ExpenseLedger MapToEntity(
            ExpenseLedgerDto dto)
        {
            return new ExpenseLedger
            {
                LedgerId = dto.LedgerId,
                EntryType = dto.EntryType,
                Category = dto.Category,
                Amount = dto.Amount,
                Description = dto.Description,
                EntryDate = dto.EntryDate,
                RecordedByAdminId = dto.RecordedByAdminId,
                TenantId = dto.TenantId,
                IsActive = dto.IsActive
            };
        }
    }
}
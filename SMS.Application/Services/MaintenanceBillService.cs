using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class MaintenanceBillService : IMaintenanceBillService
    {
        private readonly IMaintenanceBillRepository _repository;

        public MaintenanceBillService(IMaintenanceBillRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MaintenanceBillDto>> GetAllAsync()
        {
            var bills = await _repository.GetAllAsync();
            return bills.Select(MapToDto).ToList();
        }

        public async Task<MaintenanceBillDto?> GetByIdAsync(long id)
        {
            var bill = await _repository.GetByIdAsync(id);
            return bill == null ? null : MapToDto(bill);
        }

        public async Task<long> CreateAsync(MaintenanceBillDto dto)
        {
            var bill = new MaintenanceBill
            {
                FlatId = dto.FlatId,
                BillingMonth = dto.BillingMonth,
                BillingYear = dto.BillingYear,
                BaseAmount = dto.BaseAmount,
                PenaltyAmount = dto.PenaltyAmount,
                TotalAmount = dto.BaseAmount + dto.PenaltyAmount,
                DueDate = dto.DueDate.Date,
                PaymentStatus = dto.PaymentStatus.Trim(),
                PaymentDate = dto.PaymentDate?.Date,
                TransactionRef = Normalize(dto.TransactionRef),
                Description = Normalize(dto.Description),
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy,
                TenantId = dto.TenantId
            };

            return await _repository.CreateAsync(bill);
        }

        public async Task<bool> UpdateAsync(MaintenanceBillDto dto)
        {
            var bill = new MaintenanceBill
            {
                BillId = dto.BillId,
                FlatId = dto.FlatId,
                BillingMonth = dto.BillingMonth,
                BillingYear = dto.BillingYear,
                BaseAmount = dto.BaseAmount,
                PenaltyAmount = dto.PenaltyAmount,
                TotalAmount = dto.BaseAmount + dto.PenaltyAmount,
                DueDate = dto.DueDate.Date,
                PaymentStatus = dto.PaymentStatus.Trim(),
                PaymentDate = dto.PaymentDate?.Date,
                TransactionRef = Normalize(dto.TransactionRef),
                Description = Normalize(dto.Description),
                IsActive = true,
                ModifyOn = DateTime.UtcNow,
                ModifyBy = dto.ModifyBy,
                TenantId = dto.TenantId
            };

            return await _repository.UpdateAsync(bill);
        }

        public Task<bool> DeleteAsync(long id)
        {
            return _repository.DeleteAsync(id);
        }

        private static string? Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static MaintenanceBillDto MapToDto(MaintenanceBill bill)
        {
            return new MaintenanceBillDto
            {
                BillId = bill.BillId,
                FlatId = bill.FlatId,
                BillingMonth = bill.BillingMonth,
                BillingYear = bill.BillingYear,
                BaseAmount = bill.BaseAmount,
                PenaltyAmount = bill.PenaltyAmount,
                TotalAmount = bill.TotalAmount,
                DueDate = bill.DueDate,
                PaymentStatus = bill.PaymentStatus,
                PaymentDate = bill.PaymentDate,
                TransactionRef = bill.TransactionRef,
                Description = bill.Description,
                IsActive = bill.IsActive,
                CreatedOn = bill.CreatedOn,
                CreatedBy = bill.CreatedBy,
                ModifyOn = bill.ModifyOn,
                ModifyBy = bill.ModifyBy,
                TenantId = bill.TenantId
            };
        }
    }
}
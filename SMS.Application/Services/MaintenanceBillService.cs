using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class MaintenanceBillService : IMaintenanceBillService
    {
        private readonly IMaintenanceBillRepository _repository;

        public MaintenanceBillService(
            IMaintenanceBillRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MaintenanceBillDto>> GetAllAsync()
        {
            var bills = await _repository.GetAllAsync();

            return bills.Select(MapToDto).ToList();
        }

        public async Task<MaintenanceBillDto?> GetByIdAsync(long billId)
        {
            var bill = await _repository.GetByIdAsync(billId);

            return bill == null ? null : MapToDto(bill);
        }

        public async Task<List<MaintenanceBillDto>> GetByTenantAsync(
            long tenantId)
        {
            var bills = await _repository.GetByTenantAsync(tenantId);

            return bills.Select(MapToDto).ToList();
        }

        public async Task<long> CreateAsync(
            MaintenanceBillDto dto,
            long? createdBy)
        {
            var bill = MapToEntity(dto);

            bill.CreatedBy = createdBy;

            return await _repository.CreateAsync(bill);
        }

        public async Task<bool> UpdateAsync(
            MaintenanceBillDto dto,
            long? modifyBy)
        {
            var bill = MapToEntity(dto);

            bill.ModifyBy = modifyBy;

            return await _repository.UpdateAsync(bill);
        }

        public async Task<bool> DeleteAsync(
            long billId,
            long? modifyBy,
            long tenantId)
        {
            return await _repository.DeleteAsync(
                billId,
                modifyBy,
                tenantId);
        }

        private static MaintenanceBillDto MapToDto(
            MaintenanceBill bill)
        {
            return new MaintenanceBillDto
            {
                BillId = bill.BillId,
                FlatId = bill.FlatId,
                FlatNumber = bill.FlatNumber,
                BillingMonth = bill.BillingMonth,
                BillingYear = bill.BillingYear,
                BaseAmount = bill.BaseAmount,
                PenaltyAmount = bill.PenaltyAmount,
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
                TenantId = bill.TenantId,
                TenantName = bill.TenantName
            };
        }

        public async Task<List<MaintenanceBillDto>> GetFlatsByTenantAsync(long tenantId)
        {
            var flats = await _repository.GetFlatsByTenantAsync(tenantId);

            return flats.Select(flat => new MaintenanceBillDto
            {
                FlatId = flat.FlatId,
                FlatNumber = flat.FlatNumber
            }).ToList();
        }


        private static MaintenanceBill MapToEntity(
            MaintenanceBillDto dto)
        {
            return new MaintenanceBill
            {
                BillId = dto.BillId,
                FlatId = dto.FlatId,
                BillingMonth = dto.BillingMonth,
                BillingYear = dto.BillingYear,
                BaseAmount = dto.BaseAmount,
                PenaltyAmount = dto.PenaltyAmount,
                TotalAmount = dto.BaseAmount + dto.PenaltyAmount,
                DueDate = dto.DueDate,
                PaymentStatus = dto.PaymentStatus,
                PaymentDate = dto.PaymentDate,
                TransactionRef = dto.TransactionRef,
                Description = dto.Description,
                TenantId = dto.TenantId
            };
        }
    }
}
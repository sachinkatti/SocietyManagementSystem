using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;

        public InvoiceService(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
        {
            var invoices = await _repository.GetAllAsync();
            return invoices.Select(MapToDto);
        }

        public async Task<InvoiceDto?> GetByIdAsync(long invoiceId)
        {
            var invoice = await _repository.GetByIdAsync(invoiceId);
            return invoice != null ? MapToDto(invoice) : null;
        }

        public async Task<long> CreateAsync(InvoiceDto dto)
        {
            Validate(dto);

            var entity = MapToEntity(dto);
            entity.CreatedOn = DateTime.Now;
            entity.IsActive = true;

            return await _repository.CreateAsync(entity);
        }

        public async Task<bool> UpdateAsync(InvoiceDto dto)
        {
            Validate(dto);

            if (dto.InvoiceId <= 0)
            {
                throw new ArgumentException("Invalid invoice ID.");
            }

            var entity = MapToEntity(dto);
            entity.ModifyOn = DateTime.Now;

            return await _repository.UpdateAsync(entity);
        }

        public Task<bool> DeleteAsync(long invoiceId)
        {
            if (invoiceId <= 0)
            {
                throw new ArgumentException("Invalid invoice ID.");
            }

            return _repository.DeleteAsync(invoiceId);
        }

        private static InvoiceDto MapToDto(Invoice entity)
        {
            return new InvoiceDto
            {
                InvoiceId = entity.InvoiceId,
                SubscriptionId = entity.SubscriptionId,
                TenantId = entity.TenantId,
                InvoiceNumber = entity.InvoiceNumber,
                Amount = entity.Amount,
                TaxAmount = entity.TaxAmount,
                TotalAmount = entity.TotalAmount,
                InvoiceDate = entity.InvoiceDate,
                DueDate = entity.DueDate,
                Status = entity.Status,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedOn = entity.CreatedOn,
                CreatedBy = entity.CreatedBy,
                ModifyOn = entity.ModifyOn,
                ModifyBy = entity.ModifyBy
            };
        }

        private static Invoice MapToEntity(InvoiceDto dto)
        {
            return new Invoice
            {
                InvoiceId = dto.InvoiceId,
                SubscriptionId = dto.SubscriptionId,
                TenantId = dto.TenantId,
                InvoiceNumber = dto.InvoiceNumber,
                Amount = dto.Amount,
                TaxAmount = dto.TaxAmount,
                TotalAmount = dto.TotalAmount,
                InvoiceDate = dto.InvoiceDate,
                DueDate = dto.DueDate,
                Status = dto.Status,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedOn = dto.CreatedOn ?? default,
                CreatedBy = dto.CreatedBy,
                ModifyOn = dto.ModifyOn,
                ModifyBy = dto.ModifyBy
            };
        }

        private static void Validate(InvoiceDto dto)
        {
            if (dto.SubscriptionId <= 0)
            {
                throw new ArgumentException("Subscription is required.");
            }

            if (dto.TenantId <= 0)
            {
                throw new ArgumentException("Tenant is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.InvoiceNumber) || dto.InvoiceNumber.Length > 50)
            {
                throw new ArgumentException("Invoice number is required and cannot exceed 50 characters.");
            }

            if (dto.Amount < 0 || dto.TaxAmount < 0 || dto.TotalAmount < 0)
            {
                throw new ArgumentException("Amounts cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(dto.Status) || dto.Status.Length > 30)
            {
                throw new ArgumentException("Status is required and cannot exceed 30 characters.");
            }

            if (dto.Description?.Length > 500)
            {
                throw new ArgumentException("Description cannot exceed 500 characters.");
            }
        }
    }
}
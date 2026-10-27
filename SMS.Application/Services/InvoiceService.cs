using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
        {
            var invoices = await _invoiceRepository.GetAllAsync();

            return invoices.Select(MapToDto).ToList();
        }

        public async Task<InvoiceDto?> GetByIdAsync(long invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);

            if (invoice == null)
            {
                return null;
            }

            return MapToDto(invoice);
        }

        public async Task<long> CreateAsync(InvoiceDto invoiceDto)
        {
            var invoice = MapToEntity(invoiceDto);

            return await _invoiceRepository.CreateAsync(invoice);
        }

        public async Task<bool> UpdateAsync(InvoiceDto invoiceDto)
        {
            var invoice = MapToEntity(invoiceDto);

            return await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task<bool> DeleteAsync(long invoiceId)
        {
            return await _invoiceRepository.DeleteAsync(invoiceId);
        }

        private static InvoiceDto MapToDto(Invoice invoice)
        {
            return new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                SubscriptionId = invoice.SubscriptionId,
                TenantId = invoice.TenantId,
                InvoiceNumber = invoice.InvoiceNumber,
                Amount = invoice.Amount,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                Status = invoice.Status,
                Description = invoice.Description
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
                Description = dto.Description
            };
        }
    }
}

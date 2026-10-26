namespace SMS.Application
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDto>> GetAllAsync();
        Task<InvoiceDto?> GetByIdAsync(long invoiceId);
        Task<long> CreateAsync(InvoiceDto dto);
        Task<bool> UpdateAsync(InvoiceDto dto);
        Task<bool> DeleteAsync(long invoiceId);
    }
}
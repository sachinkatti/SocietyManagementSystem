namespace SMS.Application
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDto>> GetAllAsync();

        Task<InvoiceDto?> GetByIdAsync(long invoiceId);

        Task<long> CreateAsync(InvoiceDto invoiceDto);

        Task<bool> UpdateAsync(InvoiceDto invoiceDto);

        Task<bool> DeleteAsync(long invoiceId);
    }
}

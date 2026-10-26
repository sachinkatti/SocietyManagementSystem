using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(long invoiceId);
        Task<long> CreateAsync(Invoice entity);
        Task<bool> UpdateAsync(Invoice entity);
        Task<bool> DeleteAsync(long invoiceId);
    }
}
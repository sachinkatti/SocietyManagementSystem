
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetAllAsync();

        Task<Invoice?> GetByIdAsync(long invoiceId);

        Task<long> CreateAsync(Invoice invoice);

        Task<bool> UpdateAsync(Invoice invoice);

        Task<bool> DeleteAsync(long invoiceId);
    }
}

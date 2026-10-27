
using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();

        Task<Payment?> GetByIdAsync(long paymentId);

        Task<long> CreateAsync(Payment payment);

        Task<bool> UpdateAsync(Payment payment);

        Task<bool> DeleteAsync(long paymentId);
    }
}

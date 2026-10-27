namespace SMS.Application
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync();

        Task<PaymentDto?> GetByIdAsync(long paymentId);

        Task<long> CreateAsync(PaymentDto paymentDto);

        Task<bool> UpdateAsync(PaymentDto paymentDto);

        Task<bool> DeleteAsync(long paymentId);
    }
}

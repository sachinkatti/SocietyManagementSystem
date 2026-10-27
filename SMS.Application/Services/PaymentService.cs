using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();

            return payments.Select(payment => new PaymentDto
            {
                PaymentId = payment.PaymentId,
                InvoiceId = payment.InvoiceId,
                InvoiceNumber = payment.InvoiceNumber,
                PaymentReference = payment.PaymentReference,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                Description = payment.Description
            });
        }

        public async Task<PaymentDto?> GetByIdAsync(long paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            if (payment == null)
                return null;

            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                InvoiceId = payment.InvoiceId,
                InvoiceNumber = payment.InvoiceNumber,
                PaymentReference = payment.PaymentReference,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                Description = payment.Description
            };
        }

        public async Task<long> CreateAsync(PaymentDto paymentDto)
        {
            var payment = new Payment
            {
                InvoiceId = paymentDto.InvoiceId,
                PaymentReference = paymentDto.PaymentReference,
                PaymentDate = paymentDto.PaymentDate,
                Amount = paymentDto.Amount,
                PaymentMethod = paymentDto.PaymentMethod,
                Status = paymentDto.Status,
                TransactionId = paymentDto.TransactionId,
                Description = paymentDto.Description
            };

            return await _paymentRepository.CreateAsync(payment);
        }

        public async Task<bool> UpdateAsync(PaymentDto paymentDto)
        {
            var payment = new Payment
            {
                PaymentId = paymentDto.PaymentId,
                InvoiceId = paymentDto.InvoiceId,
                PaymentReference = paymentDto.PaymentReference,
                PaymentDate = paymentDto.PaymentDate,
                Amount = paymentDto.Amount,
                PaymentMethod = paymentDto.PaymentMethod,
                Status = paymentDto.Status,
                TransactionId = paymentDto.TransactionId,
                Description = paymentDto.Description
            };

            return await _paymentRepository.UpdateAsync(payment);
        }

        public async Task<bool> DeleteAsync(long paymentId)
        {
            return await _paymentRepository.DeleteAsync(paymentId);
        }
    }
}

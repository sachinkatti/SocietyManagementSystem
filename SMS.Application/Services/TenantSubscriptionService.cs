using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class TenantSubscriptionService : ITenantSubscriptionService
    {
        private readonly ITenantSubscriptionRepository _repository;

        public TenantSubscriptionService(ITenantSubscriptionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TenantSubscriptionDto>> GetAllAsync()
        {
            var subscriptions = await _repository.GetAllAsync();
            return subscriptions.Select(MapToDto);
        }

        public async Task<TenantSubscriptionDto?> GetByIdAsync(long subscriptionId)
        {
            var subscription = await _repository.GetByIdAsync(subscriptionId);
            return subscription != null ? MapToDto(subscription) : null;
        }

        public async Task<long> CreateAsync(TenantSubscriptionDto dto)
        {
            Validate(dto);

            var entity = MapToEntity(dto);
            entity.CreatedOn = DateTime.Now;
            entity.IsActive = true;

            return await _repository.CreateAsync(entity);
        }

        public async Task<bool> UpdateAsync(TenantSubscriptionDto dto)
        {
            Validate(dto);

            if (dto.SubscriptionId <= 0)
            {
                throw new ArgumentException("Invalid subscription ID.");
            }

            var entity = MapToEntity(dto);
            return await _repository.UpdateAsync(entity);
        }

        public Task<bool> DeleteAsync(long subscriptionId)
        {
            if (subscriptionId <= 0)
            {
                throw new ArgumentException("Invalid subscription ID.");
            }

            return _repository.DeleteAsync(subscriptionId);
        }

        private static TenantSubscriptionDto MapToDto(TenantSubscription entity)
        {
            return new TenantSubscriptionDto
            {
                SubscriptionId = entity.SubscriptionId,
                TenantId = entity.TenantId,
                PlanId = entity.PlanId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = entity.Status,
                AmountPaid = entity.AmountPaid,
                PaymentReference = entity.PaymentReference,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedOn = entity.CreatedOn,
                CreatedBy = entity.CreatedBy,
                ModifyOn = entity.ModifyOn,
                ModifyBy = entity.ModifyBy
            };
        }

        private static TenantSubscription MapToEntity(TenantSubscriptionDto dto)
        {
            return new TenantSubscription
            {
                SubscriptionId = dto.SubscriptionId,
                TenantId = dto.TenantId,
                PlanId = dto.PlanId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                AmountPaid = dto.AmountPaid,
                PaymentReference = dto.PaymentReference,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedOn = dto.CreatedOn ?? default,
                CreatedBy = dto.CreatedBy,
                ModifyOn = dto.ModifyOn,
                ModifyBy = dto.ModifyBy
            };
        }

        private static void Validate(TenantSubscriptionDto dto)
        {
            if (dto.TenantId <= 0)
            {
                throw new ArgumentException("Tenant is required.");
            }

            if (dto.PlanId <= 0)
            {
                throw new ArgumentException("Plan is required.");
            }

            if (dto.StartDate == default || dto.EndDate == default)
            {
                throw new ArgumentException("Start date and end date are required.");
            }

            if (dto.EndDate < dto.StartDate)
            {
                throw new ArgumentException("End date cannot be earlier than start date.");
            }

            if (string.IsNullOrWhiteSpace(dto.Status) || dto.Status.Length > 50)
            {
                throw new ArgumentException("Status is required and cannot exceed 50 characters.");
            }

            if (dto.AmountPaid < 0)
            {
                throw new ArgumentException("Amount paid cannot be negative.");
            }

            if (dto.PaymentReference?.Length > 100)
            {
                throw new ArgumentException("Payment reference cannot exceed 100 characters.");
            }

            if (dto.Description?.Length > 500)
            {
                throw new ArgumentException("Description cannot exceed 500 characters.");
            }
        }
    }
}
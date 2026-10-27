using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class TenantSubscriptionService : ITenantSubscriptionService
    {
        private readonly ITenantSubscriptionRepository _repository;

        public TenantSubscriptionService(
            ITenantSubscriptionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TenantSubscriptionDto>> GetAllAsync()
        {
            var subscriptions = await _repository.GetAllAsync();

            return subscriptions.Select(MapToDto).ToList();
        }

        public async Task<TenantSubscriptionDto?> GetByIdAsync(
            long subscriptionId)
        {
            if (subscriptionId <= 0)
                throw new ArgumentException("Invalid subscription ID.");

            var entity = await _repository.GetByIdAsync(subscriptionId);

            return entity == null ? null : MapToDto(entity);
        }

        public async Task<long> CreateAsync(TenantSubscriptionDto dto)
        {
            var entity = MapToEntity(dto);

            var validation = TenantSubscriptionValidation.Validate(entity);

            if (!validation.IsValid)
                throw new ArgumentException(validation.ErrorMessage);

            entity.IsActive = true;
            entity.CreatedOn = DateTime.Now;

            return await _repository.CreateAsync(entity);
        }

        public async Task<bool> UpdateAsync(TenantSubscriptionDto dto)
        {
            if (dto.SubscriptionId <= 0)
                throw new ArgumentException("Invalid subscription ID.");

            var entity = MapToEntity(dto);

            var validation = TenantSubscriptionValidation.Validate(entity);

            if (!validation.IsValid)
                throw new ArgumentException(validation.ErrorMessage);

            entity.ModifyOn = DateTime.Now;

            return await _repository.UpdateAsync(entity);
        }

        public Task<bool> DeleteAsync(long subscriptionId)
        {
            if (subscriptionId <= 0)
                throw new ArgumentException("Invalid subscription ID.");

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
        private static TenantSubscription MapToEntity(
            TenantSubscriptionDto dto)
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
                CreatedOn = dto.CreatedOn ?? DateTime.Now,
                CreatedBy = dto.CreatedBy,
                ModifyOn = dto.ModifyOn,
                ModifyBy = dto.ModifyBy
            };
        }
    }
}
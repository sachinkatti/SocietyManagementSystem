using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;

        public TenantService(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<IEnumerable<TenantDto>> GetAllAsync()
        {
            var tenants = await _tenantRepository.GetAllAsync();

            return tenants.Select(MapToDto).ToList();
        }

        public async Task<TenantDto?> GetByIdAsync(long tenantId)
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);

            return tenant == null ? null : MapToDto(tenant);
        }

        public async Task<long> CreateAsync(TenantDto dto)
        {
            var tenant = new Tenant
            {
                TenantName = dto.TenantName.Trim(),
                TenantCode = dto.TenantCode.Trim(),
                TenantAddress = CleanOptional(dto.TenantAddress),
                Email = CleanOptional(dto.Email),
                PhoneNumber = CleanOptional(dto.PhoneNumber),
                TenantDescription = CleanOptional(dto.TenantDescription),
                IsActive = true,
                CreatedBy = 1,
                ModifyBy = 1
            };

            return await _tenantRepository.CreateAsync(tenant);
        }

        public async Task<bool> UpdateAsync(TenantDto dto)
        {
            var tenant = new Tenant
            {
                TenantId = dto.TenantId,
                TenantName = dto.TenantName.Trim(),
                TenantCode = dto.TenantCode.Trim(),
                TenantAddress = CleanOptional(dto.TenantAddress),
                Email = CleanOptional(dto.Email),
                PhoneNumber = CleanOptional(dto.PhoneNumber),
                TenantDescription = CleanOptional(dto.TenantDescription),
                IsActive = dto.IsActive,
                ModifyBy = 1
            };

            return await _tenantRepository.UpdateAsync(tenant);
        }

        public async Task<bool> DeleteAsync(long tenantId)
        {
            return await _tenantRepository.DeleteAsync(tenantId);
        }

        private static string? CleanOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static TenantDto MapToDto(Tenant tenant)
        {
            return new TenantDto
            {
                TenantId = tenant.TenantId,
                TenantName = tenant.TenantName,
                TenantCode = tenant.TenantCode,
                TenantAddress = tenant.TenantAddress,
                Email = tenant.Email,
                PhoneNumber = tenant.PhoneNumber,
                TenantDescription = tenant.TenantDescription,
                IsActive = tenant.IsActive,
                CreatedOn = tenant.CreatedOn == default
                    ? null
                    : tenant.CreatedOn,
                CreatedBy = tenant.CreatedBy,
                ModifyOn = tenant.ModifyOn,
                ModifyBy = tenant.ModifyBy
            };
        }
    }
}
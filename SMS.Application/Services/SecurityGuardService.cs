using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class SecurityGuardService : ISecurityGuardService
    {
        private readonly ISecurityGuardRepository _repository;

        public SecurityGuardService(ISecurityGuardRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SecurityGuardDto>> GetAllAsync()
        {
            var guards = await _repository.GetAllAsync();
            return guards.Select(MapToDto);
        }

        public async Task<SecurityGuardDto?> GetByIdAsync(long guardId)
        {
            var guard = await _repository.GetByIdAsync(guardId);
            return guard == null ? null : MapToDto(guard);
        }

        public Task<long> CreateAsync(SecurityGuardDto dto)
        {
            return _repository.CreateAsync(MapToEntity(dto));
        }

        public Task<bool> UpdateAsync(SecurityGuardDto dto)
        {
            return _repository.UpdateAsync(MapToEntity(dto));
        }

        public Task<bool> DeleteAsync(long guardId)
        {
            return _repository.DeleteAsync(guardId);
        }

        private static SecurityGuardDto MapToDto(SecurityGuard guard)
        {
            return new SecurityGuardDto
            {
                GuardId = guard.GuardId,
                UserId = guard.UserId,
                ShiftTiming = guard.ShiftTiming,
                AssignedGate = guard.AssignedGate,
                JoiningDate = guard.JoiningDate,
                IsActive = guard.IsActive,
                CreatedOn = guard.CreatedOn,
                CreatedBy = guard.CreatedBy,
                ModifyOn = guard.ModifyOn,
                ModifyBy = guard.ModifyBy
            };
        }

        private static SecurityGuard MapToEntity(SecurityGuardDto dto)
        {
            return new SecurityGuard
            {
                GuardId = dto.GuardId,
                UserId = dto.UserId,
                ShiftTiming = dto.ShiftTiming,
                AssignedGate = dto.AssignedGate,
                JoiningDate = dto.JoiningDate,
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy,
                ModifyBy = dto.ModifyBy
            };
        }
    }
}

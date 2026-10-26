using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class VisitorLogService : IVisitorLogService
    {
        private readonly IVisitorLogRepository _repository;

        public VisitorLogService(IVisitorLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<VisitorLogDto>> GetAllAsync()
        {
            var visitorLogs = await _repository.GetAllAsync();

            return visitorLogs.Select(MapToDto).ToList();
        }

        public async Task<VisitorLogDto?> GetByIdAsync(long visitorId)
        {
            var visitorLog = await _repository.GetByIdAsync(visitorId);

            return visitorLog == null ? null : MapToDto(visitorLog);
        }

        public async Task<long> CreateAsync(VisitorLogDto dto)
        {
            var visitorLog = MapToEntity(dto);
            visitorLog.IsActive = true;
            visitorLog.CreatedOn = DateTime.UtcNow;

            return await _repository.CreateAsync(visitorLog);
        }

        public async Task<bool> UpdateAsync(VisitorLogDto dto)
        {
            var visitorLog = MapToEntity(dto);
            visitorLog.ModifyOn = DateTime.UtcNow;

            return await _repository.UpdateAsync(visitorLog);
        }

        public Task<bool> DeleteAsync(long visitorId, long? modifyBy)
        {
            return _repository.DeleteAsync(visitorId, modifyBy);
        }

        private static VisitorLogDto MapToDto(VisitorLog visitorLog)
        {
            return new VisitorLogDto
            {
                VisitorId = visitorLog.VisitorId,
                VisitorName = visitorLog.VisitorName,
                PhoneNumber = visitorLog.PhoneNumber,
                VisitingFlatId = visitorLog.VisitingFlatId,
                Purpose = visitorLog.Purpose,
                VehicleNumber = visitorLog.VehicleNumber,
                ApprovalMethod = visitorLog.ApprovalMethod,
                EntryTime = visitorLog.EntryTime,
                ExitTime = visitorLog.ExitTime,
                LoggedByGuardId = visitorLog.LoggedByGuardId,
                Description = visitorLog.Description,
                IsActive = visitorLog.IsActive,
                CreatedOn = visitorLog.CreatedOn,
                CreatedBy = visitorLog.CreatedBy,
                ModifyOn = visitorLog.ModifyOn,
                ModifyBy = visitorLog.ModifyBy,
                TenantId = visitorLog.TenantId
            };
        }

        private static VisitorLog MapToEntity(VisitorLogDto dto)
        {
            return new VisitorLog
            {
                VisitorId = dto.VisitorId,
                VisitorName = dto.VisitorName,
                PhoneNumber = dto.PhoneNumber,
                VisitingFlatId = dto.VisitingFlatId,
                Purpose = dto.Purpose,
                VehicleNumber = dto.VehicleNumber,
                ApprovalMethod = dto.ApprovalMethod,
                EntryTime = dto.EntryTime,
                ExitTime = dto.ExitTime,
                LoggedByGuardId = dto.LoggedByGuardId,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedOn = dto.CreatedOn,
                CreatedBy = dto.CreatedBy,
                ModifyOn = dto.ModifyOn,
                ModifyBy = dto.ModifyBy,
                TenantId = dto.TenantId
            };
        }
    }
}
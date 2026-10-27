using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class VisitorLogService : IVisitorLogService
    {
        private readonly IVisitorLogRepository _visitorLogRepository;

        public VisitorLogService(
            IVisitorLogRepository visitorLogRepository)
        {
            _visitorLogRepository = visitorLogRepository;
        }

        // 1. Get all visitor logs
        public async Task<IEnumerable<VisitorLogDto>> GetAllAsync()
        {
            var visitorLogs =
                await _visitorLogRepository.GetAllAsync();

            return visitorLogs.Select(MapToDto).ToList();
        }

        // 2. Get visitor log by ID
        public async Task<VisitorLogDto?> GetByIdAsync(long visitorId)
        {
            var visitorLog =
                await _visitorLogRepository.GetByIdAsync(visitorId);

            return visitorLog == null
                ? null
                : MapToDto(visitorLog);
        }

        // 3. Create visitor log
        public async Task<long> CreateAsync(
            VisitorLogDto visitorLogDto)
        {
            var visitorLog = MapToEntity(visitorLogDto);

            return await _visitorLogRepository.CreateAsync(visitorLog);
        }

        // 4. Update visitor log
        public async Task<bool> UpdateAsync(
            VisitorLogDto visitorLogDto)
        {
            var visitorLog = MapToEntity(visitorLogDto);

            return await _visitorLogRepository.UpdateAsync(visitorLog);
        }

        // 5. Soft-delete visitor log
        public async Task<bool> DeleteAsync(
            long visitorId, long? modifyBy = null)
        {
            return await _visitorLogRepository.DeleteAsync(
                visitorId, modifyBy);
        }

        // 6. Get flats for a tenant
        public async Task<IEnumerable<VisitorLogFlatOption>>
            GetFlatsByTenantAsync(long tenantId)
        {
            return await _visitorLogRepository.GetFlatsByTenantAsync(
                tenantId);
        }

        // 7. Get guards for a tenant
        public async Task<IEnumerable<VisitorLogGuardOption>>
            GetGuardsByTenantAsync(long tenantId)
        {
            return await _visitorLogRepository.GetGuardsByTenantAsync(
                tenantId);
        }

        // Convert entity to DTO
        private static VisitorLogDto MapToDto(VisitorLog visitorLog)
        {
            return new VisitorLogDto
            {
                VisitorId = visitorLog.VisitorId,
                VisitorName = visitorLog.VisitorName,
                PhoneNumber = visitorLog.PhoneNumber,
                VisitingFlatId = visitorLog.VisitingFlatId,
                FlatNumber = visitorLog.FlatNumber,
                WingName = visitorLog.WingName,
                Purpose = visitorLog.Purpose,
                VehicleNumber = visitorLog.VehicleNumber,
                ApprovalMethod = visitorLog.ApprovalMethod,
                EntryTime = visitorLog.EntryTime,
                ExitTime = visitorLog.ExitTime,
                LoggedByGuardId = visitorLog.LoggedByGuardId,
                GuardUserId = visitorLog.GuardUserId,
                GuardName = visitorLog.GuardName,
                Description = visitorLog.Description,
                IsActive = visitorLog.IsActive,
                CreatedOn = visitorLog.CreatedOn,
                CreatedBy = visitorLog.CreatedBy,
                ModifyOn = visitorLog.ModifyOn,
                ModifyBy = visitorLog.ModifyBy,
                TenantId = visitorLog.TenantId,
                TenantName = visitorLog.TenantName
            };
        }

        // Convert DTO to entity
        private static VisitorLog MapToEntity(
            VisitorLogDto visitorLogDto)
        {
            return new VisitorLog
            {
                VisitorId = visitorLogDto.VisitorId,
                VisitorName = visitorLogDto.VisitorName,
                PhoneNumber = visitorLogDto.PhoneNumber,
                VisitingFlatId = visitorLogDto.VisitingFlatId,
                Purpose = visitorLogDto.Purpose,
                VehicleNumber = visitorLogDto.VehicleNumber,
                ApprovalMethod = visitorLogDto.ApprovalMethod,
                EntryTime = visitorLogDto.EntryTime,
                ExitTime = visitorLogDto.ExitTime,
                LoggedByGuardId = visitorLogDto.LoggedByGuardId,
                Description = visitorLogDto.Description,
                IsActive = visitorLogDto.IsActive,
                CreatedBy = visitorLogDto.CreatedBy,
                ModifyBy = visitorLogDto.ModifyBy,
                TenantId = visitorLogDto.TenantId
            };
        }
    }
}

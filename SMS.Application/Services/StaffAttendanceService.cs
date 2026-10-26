using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class StaffAttendanceService : IStaffAttendanceService
    {
        private readonly IStaffAttendanceRepository _repository;

        public StaffAttendanceService(IStaffAttendanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StaffAttendanceDto>> GetAllAsync()
        {
            var records = await _repository.GetAllAsync();
            return records.Select(MapToDto).ToList();
        }

        public async Task<StaffAttendanceDto?> GetByIdAsync(long id)
        {
            var record = await _repository.GetByIdAsync(id);
            return record == null ? null : MapToDto(record);
        }

        public async Task<long> CreateAsync(StaffAttendanceDto dto)
        {
            var attendance = new StaffAttendance
            {
                StaffName = dto.StaffName.Trim(),
                StaffRole = dto.StaffRole.Trim(),
                FlatId = dto.FlatId,
                CheckInTime = dto.CheckInTime,
                CheckOutTime = dto.CheckOutTime,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy,
                TenantId = dto.TenantId
            };

            return await _repository.CreateAsync(attendance);
        }

        public async Task<bool> UpdateAsync(StaffAttendanceDto dto)
        {
            var attendance = new StaffAttendance
            {
                AttendanceId = dto.AttendanceId,
                StaffName = dto.StaffName.Trim(),
                StaffRole = dto.StaffRole.Trim(),
                FlatId = dto.FlatId,
                CheckInTime = dto.CheckInTime,
                CheckOutTime = dto.CheckOutTime,
                IsActive = true,
                ModifyOn = DateTime.UtcNow,
                ModifyBy = dto.ModifyBy,
                TenantId = dto.TenantId
            };

            return await _repository.UpdateAsync(attendance);
        }

        public Task<bool> DeleteAsync(long id)
        {
            return _repository.DeleteAsync(id);
        }

        private static StaffAttendanceDto MapToDto(StaffAttendance attendance)
        {
            return new StaffAttendanceDto
            {
                AttendanceId = attendance.AttendanceId,
                StaffName = attendance.StaffName,
                StaffRole = attendance.StaffRole,
                FlatId = attendance.FlatId,
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
                IsActive = attendance.IsActive,
                CreatedOn = attendance.CreatedOn,
                CreatedBy = attendance.CreatedBy,
                ModifyOn = attendance.ModifyOn,
                ModifyBy = attendance.ModifyBy,
                TenantId = attendance.TenantId
            };
        }
    }
}
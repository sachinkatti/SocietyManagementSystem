using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class StaffAttendanceService : IStaffAttendanceService
    {
        private readonly IStaffAttendanceRepository _repository;

        public StaffAttendanceService(
            IStaffAttendanceRepository repository)
        {
            _repository = repository;
        }

        // 1. Get all attendance records
        public async Task<List<StaffAttendanceDto>> GetAllAsync()
        {
            var attendanceList = await _repository.GetAllAsync();

            return attendanceList.Select(MapToDto).ToList();
        }

        // 2. Get attendance record by ID
        public async Task<StaffAttendanceDto?> GetByIdAsync(
            long attendanceId)
        {
            var attendance = await _repository.GetByIdAsync(attendanceId);

            if (attendance == null)
            {
                return null;
            }

            return MapToDto(attendance);
        }

        // 3. Create attendance record
        public async Task<long> CreateAsync(StaffAttendanceDto dto)
        {
            ValidateAttendance(dto);

            var attendance = new StaffAttendance
            {
                StaffName = dto.StaffName.Trim(),
                StaffRole = dto.StaffRole.Trim(),
                FlatId = dto.FlatId,
                CheckInTime = dto.CheckInTime,
                CheckOutTime = dto.CheckOutTime,
                CreatedBy = dto.CreatedBy,
                TenantId = dto.TenantId,
                IsActive = true
            };

            return await _repository.CreateAsync(attendance);
        }

        // 4. Update attendance record
        public async Task<bool> UpdateAsync(StaffAttendanceDto dto)
        {
            ValidateAttendance(dto);

            var attendance = new StaffAttendance
            {
                AttendanceId = dto.AttendanceId,
                StaffName = dto.StaffName.Trim(),
                StaffRole = dto.StaffRole.Trim(),
                FlatId = dto.FlatId,
                CheckInTime = dto.CheckInTime,
                CheckOutTime = dto.CheckOutTime,
                ModifyBy = dto.ModifyBy,
                TenantId = dto.TenantId
            };

            return await _repository.UpdateAsync(attendance);
        }

        // 5. Soft delete attendance record
        public async Task<bool> DeleteAsync(
            long attendanceId, long? modifyBy)
        {
            return await _repository.DeleteAsync(attendanceId, modifyBy);
        }

        // 6. Get flats for a selected society
        public async Task<List<StaffAttendance>> GetFlatsByTenantAsync(
            long tenantId)
        {
            return await _repository.GetFlatsByTenantAsync(tenantId);
        }

        // Convert entity to DTO
        private static StaffAttendanceDto MapToDto(
            StaffAttendance attendance)
        {
            return new StaffAttendanceDto
            {
                AttendanceId = attendance.AttendanceId,
                StaffName = attendance.StaffName,
                StaffRole = attendance.StaffRole,
                FlatId = attendance.FlatId,
                FlatNumber = attendance.FlatNumber,
                WingName = attendance.WingName,
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
                IsActive = attendance.IsActive,
                CreatedOn = attendance.CreatedOn,
                CreatedBy = attendance.CreatedBy,
                ModifyOn = attendance.ModifyOn,
                ModifyBy = attendance.ModifyBy,
                TenantId = attendance.TenantId,
                TenantName = attendance.TenantName
            };
        }

        // Basic business validation
        private static void ValidateAttendance(StaffAttendanceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.StaffName))
            {
                throw new ArgumentException("Staff name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.StaffRole))
            {
                throw new ArgumentException("Staff role is required.");
            }

            if (dto.FlatId <= 0)
            {
                throw new ArgumentException("Please select a valid flat.");
            }

            if (dto.TenantId <= 0)
            {
                throw new ArgumentException("Please select a valid society.");
            }

            if (dto.CheckInTime == default)
            {
                throw new ArgumentException("Check-in time is required.");
            }

            if (dto.CheckOutTime.HasValue &&
                dto.CheckOutTime.Value < dto.CheckInTime)
            {
                throw new ArgumentException(
                    "Check-out time cannot be earlier than check-in time.");
            }
        }
    }
}

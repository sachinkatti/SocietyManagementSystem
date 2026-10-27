
using SMS.Domain;

namespace SMS.Application
{
    public interface IStaffAttendanceService
    {
        Task<List<StaffAttendanceDto>> GetAllAsync();

        Task<StaffAttendanceDto?> GetByIdAsync(long attendanceId);

        Task<long> CreateAsync(StaffAttendanceDto dto);

        Task<bool> UpdateAsync(StaffAttendanceDto dto);

        Task<bool> DeleteAsync(long attendanceId, long? modifyBy);

        Task<List<StaffAttendance>> GetFlatsByTenantAsync(long tenantId);
    }
}

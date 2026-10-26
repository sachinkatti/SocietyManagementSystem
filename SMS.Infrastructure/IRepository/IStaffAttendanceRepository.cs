using SMS.Domain;

namespace SMS.Infrastructure
{
    public interface IStaffAttendanceRepository
    {
        Task<List<StaffAttendance>> GetAllAsync();
        Task<StaffAttendance?> GetByIdAsync(long id);
        Task<long> CreateAsync(StaffAttendance attendance);
        Task<bool> UpdateAsync(StaffAttendance attendance);
        Task<bool> DeleteAsync(long id);
    }
}
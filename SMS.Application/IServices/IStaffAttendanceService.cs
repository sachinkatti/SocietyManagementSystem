namespace SMS.Application
{
    public interface IStaffAttendanceService
    {
        Task<List<StaffAttendanceDto>> GetAllAsync();
        Task<StaffAttendanceDto?> GetByIdAsync(long id);
        Task<long> CreateAsync(StaffAttendanceDto dto);
        Task<bool> UpdateAsync(StaffAttendanceDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
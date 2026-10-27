
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class StaffAttendanceRepository : IStaffAttendanceRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public StaffAttendanceRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // 1. Get all active attendance records
        public async Task<List<StaffAttendance>> GetAllAsync()
        {
            var attendanceList = new List<StaffAttendance>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.GetAllStaffAttendance", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                attendanceList.Add(MapStaffAttendance(reader));
            }

            return attendanceList;
        }

        // 2. Get attendance by ID
        public async Task<StaffAttendance?> GetByIdAsync(long attendanceId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.GetStaffAttendanceById", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@AttendanceId", SqlDbType.BigInt)
                .Value = attendanceId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapStaffAttendance(reader);
            }

            return null;
        }

        // 3. Insert attendance record
        public async Task<long> CreateAsync(StaffAttendance attendance)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.InsertStaffAttendance", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@StaffName", SqlDbType.NVarChar, 100)
                .Value = attendance.StaffName;

            command.Parameters.Add("@StaffRole", SqlDbType.NVarChar, 50)
                .Value = attendance.StaffRole;

            command.Parameters.Add("@FlatId", SqlDbType.BigInt)
                .Value = attendance.FlatId;

            command.Parameters.Add("@CheckInTime", SqlDbType.DateTime2)
                .Value = attendance.CheckInTime;

            command.Parameters.Add("@CheckOutTime", SqlDbType.DateTime2)
                .Value = attendance.CheckOutTime.HasValue
                    ? attendance.CheckOutTime.Value
                    : DBNull.Value;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt)
                .Value = attendance.CreatedBy.HasValue
                    ? attendance.CreatedBy.Value
                    : DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt)
                .Value = attendance.TenantId;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        // 4. Update attendance record
        public async Task<bool> UpdateAsync(StaffAttendance attendance)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.UpdateStaffAttendance", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@AttendanceId", SqlDbType.BigInt)
                .Value = attendance.AttendanceId;

            command.Parameters.Add("@StaffName", SqlDbType.NVarChar, 100)
                .Value = attendance.StaffName;

            command.Parameters.Add("@StaffRole", SqlDbType.NVarChar, 50)
                .Value = attendance.StaffRole;

            command.Parameters.Add("@FlatId", SqlDbType.BigInt)
                .Value = attendance.FlatId;

            command.Parameters.Add("@CheckInTime", SqlDbType.DateTime2)
                .Value = attendance.CheckInTime;

            command.Parameters.Add("@CheckOutTime", SqlDbType.DateTime2)
                .Value = attendance.CheckOutTime.HasValue
                    ? attendance.CheckOutTime.Value
                    : DBNull.Value;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt)
                .Value = attendance.ModifyBy.HasValue
                    ? attendance.ModifyBy.Value
                    : DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt)
                .Value = attendance.TenantId;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        // 5. Soft delete attendance record
        public async Task<bool> DeleteAsync(long attendanceId, long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.DeleteStaffAttendance", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@AttendanceId", SqlDbType.BigInt)
                .Value = attendanceId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt)
                .Value = modifyBy.HasValue
                    ? modifyBy.Value
                    : DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        // 6. Get flats belonging to the selected society
        public async Task<List<StaffAttendance>> GetFlatsByTenantAsync(
            long tenantId)
        {
            var flats = new List<StaffAttendance>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.GetStaffAttendanceFlatsByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt)
                .Value = tenantId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                flats.Add(new StaffAttendance
                {
                    FlatId = Convert.ToInt64(reader["FlatId"]),
                    FlatNumber = reader["FlatNumber"]?.ToString(),
                    WingName = reader["WingName"]?.ToString(),
                    TenantId = tenantId
                });
            }

            return flats;
        }

        // Common method to map SQL results to the entity
        private static StaffAttendance MapStaffAttendance(
            SqlDataReader reader)
        {
            return new StaffAttendance
            {
                AttendanceId = Convert.ToInt64(reader["AttendanceId"]),
                StaffName = reader["StaffName"]?.ToString() ?? string.Empty,
                StaffRole = reader["StaffRole"]?.ToString() ?? string.Empty,
                FlatId = Convert.ToInt64(reader["FlatId"]),
                FlatNumber = reader["FlatNumber"]?.ToString(),
                WingName = reader["WingName"]?.ToString(),
                CheckInTime = Convert.ToDateTime(reader["CheckInTime"]),

                CheckOutTime = reader["CheckOutTime"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["CheckOutTime"]),

                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),

                CreatedBy = reader["CreatedBy"] == DBNull.Value
                    ? null
                    : Convert.ToInt64(reader["CreatedBy"]),

                ModifyOn = reader["ModifyOn"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["ModifyOn"]),

                ModifyBy = reader["ModifyBy"] == DBNull.Value
                    ? null
                    : Convert.ToInt64(reader["ModifyBy"]),

                TenantId = Convert.ToInt64(reader["TenantId"]),

                TenantName = reader["TenantName"]?.ToString()
            };
        }
    }
}
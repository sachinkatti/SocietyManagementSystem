using Microsoft.Data.SqlClient;
using SMS.Domain;

namespace SMS.Infrastructure
{
    public class StaffAttendanceRepository : IStaffAttendanceRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public StaffAttendanceRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<StaffAttendance>> GetAllAsync()
        {
            var attendanceList = new List<StaffAttendance>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT AttendanceId, StaffName, StaffRole, FlatId,
                       CheckInTime, CheckOutTime, IsActive, CreatedOn,
                       CreatedBy, ModifyOn, ModifyBy, TenantId
                FROM StaffAttendance
                WHERE IsActive = 1
                ORDER BY AttendanceId DESC";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                attendanceList.Add(MapAttendance(reader));
            }

            return attendanceList;
        }

        public async Task<StaffAttendance?> GetByIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT AttendanceId, StaffName, StaffRole, FlatId,
                       CheckInTime, CheckOutTime, IsActive, CreatedOn,
                       CreatedBy, ModifyOn, ModifyBy, TenantId
                FROM StaffAttendance
                WHERE AttendanceId = @AttendanceId AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AttendanceId", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapAttendance(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(StaffAttendance attendance)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO StaffAttendance
                (
                    StaffName, StaffRole, FlatId, CheckInTime,
                    CheckOutTime, IsActive, CreatedOn, CreatedBy,
                    ModifyOn, ModifyBy, TenantId
                )
                VALUES
                (
                    @StaffName, @StaffRole, @FlatId, @CheckInTime,
                    @CheckOutTime, @IsActive, @CreatedOn, @CreatedBy,
                    @ModifyOn, @ModifyBy, @TenantId
                );

                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using var command = new SqlCommand(query, connection);
            AddParameters(command, attendance);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(StaffAttendance attendance)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                UPDATE StaffAttendance
                SET StaffName = @StaffName,
                    StaffRole = @StaffRole,
                    FlatId = @FlatId,
                    CheckInTime = @CheckInTime,
                    CheckOutTime = @CheckOutTime,
                    ModifyOn = @ModifyOn,
                    ModifyBy = @ModifyBy,
                    TenantId = @TenantId
                WHERE AttendanceId = @AttendanceId AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            AddParameters(command, attendance);
            command.Parameters.AddWithValue("@AttendanceId", attendance.AttendanceId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                UPDATE StaffAttendance
                SET IsActive = 0,
                    ModifyOn = @ModifyOn
                WHERE AttendanceId = @AttendanceId AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AttendanceId", id);
            command.Parameters.AddWithValue("@ModifyOn", DateTime.UtcNow);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static void AddParameters(
            SqlCommand command,
            StaffAttendance attendance)
        {
            command.Parameters.AddWithValue("@StaffName", attendance.StaffName);
            command.Parameters.AddWithValue("@StaffRole", attendance.StaffRole);
            command.Parameters.AddWithValue("@FlatId", attendance.FlatId);
            command.Parameters.AddWithValue("@CheckInTime", attendance.CheckInTime);

            command.Parameters.AddWithValue(
                "@CheckOutTime",
                (object?)attendance.CheckOutTime ?? DBNull.Value);

            command.Parameters.AddWithValue("@IsActive", attendance.IsActive);
            command.Parameters.AddWithValue("@CreatedOn", attendance.CreatedOn);

            command.Parameters.AddWithValue(
                "@CreatedBy",
                (object?)attendance.CreatedBy ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@ModifyOn",
                (object?)attendance.ModifyOn ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@ModifyBy",
                (object?)attendance.ModifyBy ?? DBNull.Value);

            command.Parameters.AddWithValue("@TenantId", attendance.TenantId);
        }

        private static StaffAttendance MapAttendance(SqlDataReader reader)
        {
            return new StaffAttendance
            {
                AttendanceId = reader.GetInt64(reader.GetOrdinal("AttendanceId")),
                StaffName = reader.GetString(reader.GetOrdinal("StaffName")),
                StaffRole = reader.GetString(reader.GetOrdinal("StaffRole")),
                FlatId = reader.GetInt64(reader.GetOrdinal("FlatId")),
                CheckInTime = reader.GetDateTime(reader.GetOrdinal("CheckInTime")),
                CheckOutTime = reader.IsDBNull(reader.GetOrdinal("CheckOutTime"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CheckOutTime")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null
                    : reader.GetInt64(reader.GetOrdinal("CreatedBy")),
                ModifyOn = reader.IsDBNull(reader.GetOrdinal("ModifyOn"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("ModifyOn")),
                ModifyBy = reader.IsDBNull(reader.GetOrdinal("ModifyBy"))
                    ? null
                    : reader.GetInt64(reader.GetOrdinal("ModifyBy")),
                TenantId = reader.GetInt64(reader.GetOrdinal("TenantId"))
            };
        }
    }
}
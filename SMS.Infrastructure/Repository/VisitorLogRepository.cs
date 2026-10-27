
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class VisitorLogRepository : IVisitorLogRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public VisitorLogRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // 1. Get all active visitor logs
        public async Task<IEnumerable<VisitorLog>> GetAllAsync()
        {
            var visitorLogs = new List<VisitorLog>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "GetAllVisitorLogs", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                visitorLogs.Add(MapVisitorLog(reader));
            }

            return visitorLogs;
        }

        // 2. Get visitor log by ID
        public async Task<VisitorLog?> GetByIdAsync(long visitorId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "GetVisitorLogById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@VisitorId", SqlDbType.BigInt).Value =
                visitorId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapVisitorLog(reader);
            }

            return null;
        }

        // 3. Insert visitor log
        public async Task<long> CreateAsync(VisitorLog visitorLog)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "InsertVisitorLog", connection);

            command.CommandType = CommandType.StoredProcedure;

            AddVisitorLogParameters(command, visitorLog);

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)visitorLog.CreatedBy ?? DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                (object?)visitorLog.TenantId ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        // 4. Update visitor log
        public async Task<bool> UpdateAsync(VisitorLog visitorLog)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UpdateVisitorLog", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@VisitorId", SqlDbType.BigInt).Value =
                visitorLog.VisitorId;

            AddVisitorLogParameters(command, visitorLog);

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)visitorLog.ModifyBy ?? DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                (object?)visitorLog.TenantId ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        // 5. Soft-delete visitor log
        public async Task<bool> DeleteAsync(
            long visitorId, long? modifyBy = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "DeleteVisitorLog", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@VisitorId", SqlDbType.BigInt).Value =
                visitorId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        // 6. Get active flats for a tenant
        public async Task<IEnumerable<VisitorLogFlatOption>>
            GetFlatsByTenantAsync(long tenantId)
        {
            var flats = new List<VisitorLogFlatOption>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "GetVisitorLogFlatsByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                tenantId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                flats.Add(new VisitorLogFlatOption
                {
                    FlatId = reader.GetInt64(
                        reader.GetOrdinal("FlatId")),

                    FlatNumber = reader["FlatNumber"]?.ToString()
                        ?? string.Empty,

                    WingId = reader.GetInt64(
                        reader.GetOrdinal("WingId")),

                    WingName = reader["WingName"]?.ToString()
                        ?? string.Empty
                });
            }

            return flats;
        }

        // 7. Get active guards for a tenant
        public async Task<IEnumerable<VisitorLogGuardOption>>
            GetGuardsByTenantAsync(long tenantId)
        {
            var guards = new List<VisitorLogGuardOption>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "GetVisitorLogGuardsByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                tenantId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                guards.Add(new VisitorLogGuardOption
                {
                    GuardId = reader.GetInt64(
                        reader.GetOrdinal("GuardId")),

                    UserId = reader.GetInt64(
                        reader.GetOrdinal("UserId")),

                    GuardName = reader["GuardName"]?.ToString()
                        ?? string.Empty
                });
            }

            return guards;
        }

        // Common parameters for insert and update
        private static void AddVisitorLogParameters(
            SqlCommand command, VisitorLog visitorLog)
        {
            command.Parameters.Add(
                "@VisitorName", SqlDbType.NVarChar, 100).Value =
                visitorLog.VisitorName;

            command.Parameters.Add(
                "@PhoneNumber", SqlDbType.NVarChar, 15).Value =
                (object?)visitorLog.PhoneNumber ?? DBNull.Value;

            command.Parameters.Add(
                "@VisitingFlatId", SqlDbType.BigInt).Value =
                visitorLog.VisitingFlatId;

            command.Parameters.Add(
                "@Purpose", SqlDbType.NVarChar, 50).Value =
                visitorLog.Purpose;

            command.Parameters.Add(
                "@VehicleNumber", SqlDbType.NVarChar, 20).Value =
                (object?)visitorLog.VehicleNumber ?? DBNull.Value;

            command.Parameters.Add(
                "@ApprovalMethod", SqlDbType.NVarChar, 20).Value =
                visitorLog.ApprovalMethod;

            command.Parameters.Add(
                "@EntryTime", SqlDbType.DateTime2).Value =
                visitorLog.EntryTime;

            command.Parameters.Add(
                "@ExitTime", SqlDbType.DateTime2).Value =
                (object?)visitorLog.ExitTime ?? DBNull.Value;

            command.Parameters.Add(
                "@LoggedByGuardId", SqlDbType.BigInt).Value =
                (object?)visitorLog.LoggedByGuardId ?? DBNull.Value;

            command.Parameters.Add(
                "@Description", SqlDbType.NVarChar, 500).Value =
                (object?)visitorLog.Description ?? DBNull.Value;
        }

        // Convert database row into VisitorLog entity
        private static VisitorLog MapVisitorLog(SqlDataReader reader)
        {
            return new VisitorLog
            {
                VisitorId = reader.GetInt64(
                    reader.GetOrdinal("VisitorId")),

                VisitorName = reader["VisitorName"]?.ToString()
                    ?? string.Empty,

                PhoneNumber = GetNullableString(reader, "PhoneNumber"),

                VisitingFlatId = reader.GetInt64(
                    reader.GetOrdinal("VisitingFlatId")),

                FlatNumber = GetNullableString(reader, "FlatNumber"),

                WingName = GetNullableString(reader, "WingName"),

                Purpose = reader["Purpose"]?.ToString()
                    ?? string.Empty,

                VehicleNumber = GetNullableString(
                    reader, "VehicleNumber"),

                ApprovalMethod = reader["ApprovalMethod"]?.ToString()
                    ?? string.Empty,

                EntryTime = reader.GetDateTime(
                    reader.GetOrdinal("EntryTime")),

                ExitTime = GetNullableDateTime(reader, "ExitTime"),

                LoggedByGuardId = GetNullableInt64(
                    reader, "LoggedByGuardId"),

                GuardUserId = GetNullableInt64(
                    reader, "GuardUserId"),

                GuardName = GetNullableString(reader, "GuardName"),

                Description = GetNullableString(reader, "Description"),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreatedOn = reader.GetDateTime(
                    reader.GetOrdinal("CreatedOn")),

                CreatedBy = GetNullableInt64(reader, "CreatedBy"),

                ModifyOn = GetNullableDateTime(reader, "ModifyOn"),

                ModifyBy = GetNullableInt64(reader, "ModifyBy"),

                TenantId = GetNullableInt64(reader, "TenantId"),

                TenantName = GetNullableString(reader, "TenantName")
            };
        }

        private static string? GetNullableString(
            SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            return reader.IsDBNull(ordinal)
                ? null
                : reader.GetString(ordinal);
        }

        private static long? GetNullableInt64(
            SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            return reader.IsDBNull(ordinal)
                ? null
                : reader.GetInt64(ordinal);
        }

        private static DateTime? GetNullableDateTime(
            SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            return reader.IsDBNull(ordinal)
                ? null
                : reader.GetDateTime(ordinal);
        }
    }
}

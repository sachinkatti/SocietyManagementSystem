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

        public async Task<List<VisitorLog>> GetAllAsync()
        {
            var visitorLogs = new List<VisitorLog>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT VisitorId, VisitorName, PhoneNumber, VisitingFlatId,
                         Purpose, VehicleNumber, ApprovalMethod, EntryTime, ExitTime,
                         LoggedByGuardId, Description, IsActive, CreatedOn, CreatedBy,
                         ModifyOn, ModifyBy, TenantId
                  FROM VisitorLogs
                  WHERE IsActive = 1
                  ORDER BY VisitorId DESC",
                connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                visitorLogs.Add(MapVisitorLog(reader));
            }

            return visitorLogs;
        }

        public async Task<VisitorLog?> GetByIdAsync(long visitorId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT VisitorId, VisitorName, PhoneNumber, VisitingFlatId,
                         Purpose, VehicleNumber, ApprovalMethod, EntryTime, ExitTime,
                         LoggedByGuardId, Description, IsActive, CreatedOn, CreatedBy,
                         ModifyOn, ModifyBy, TenantId
                  FROM VisitorLogs
                  WHERE VisitorId = @VisitorId AND IsActive = 1",
                connection);

            command.Parameters.Add("@VisitorId", SqlDbType.BigInt).Value = visitorId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapVisitorLog(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(VisitorLog visitorLog)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"INSERT INTO VisitorLogs
                  (VisitorName, PhoneNumber, VisitingFlatId, Purpose, VehicleNumber,
                   ApprovalMethod, EntryTime, ExitTime, LoggedByGuardId, Description,
                   IsActive, CreatedOn, CreatedBy, TenantId)
                  VALUES
                  (@VisitorName, @PhoneNumber, @VisitingFlatId, @Purpose, @VehicleNumber,
                   @ApprovalMethod, @EntryTime, @ExitTime, @LoggedByGuardId, @Description,
                   @IsActive, @CreatedOn, @CreatedBy, @TenantId);
                  SELECT CAST(SCOPE_IDENTITY() AS BIGINT);",
                connection);

            AddEditableParameters(command, visitorLog);
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = true;
            command.Parameters.Add("@CreatedOn", SqlDbType.DateTime2).Value = visitorLog.CreatedOn;
            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)visitorLog.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();
            return Convert.ToInt64(await command.ExecuteScalarAsync());
        }

        public async Task<bool> UpdateAsync(VisitorLog visitorLog)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"UPDATE VisitorLogs
                  SET VisitorName = @VisitorName,
                      PhoneNumber = @PhoneNumber,
                      VisitingFlatId = @VisitingFlatId,
                      Purpose = @Purpose,
                      VehicleNumber = @VehicleNumber,
                      ApprovalMethod = @ApprovalMethod,
                      EntryTime = @EntryTime,
                      ExitTime = @ExitTime,
                      LoggedByGuardId = @LoggedByGuardId,
                      Description = @Description,
                      TenantId = @TenantId,
                      ModifyOn = @ModifyOn,
                      ModifyBy = @ModifyBy
                  WHERE VisitorId = @VisitorId AND IsActive = 1",
                connection);

            AddEditableParameters(command, visitorLog);
            command.Parameters.Add("@VisitorId", SqlDbType.BigInt).Value = visitorLog.VisitorId;
            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime2).Value =
                (object?)visitorLog.ModifyOn ?? DBNull.Value;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)visitorLog.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long visitorId, long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"UPDATE VisitorLogs
                  SET IsActive = 0, ModifyOn = @ModifyOn, ModifyBy = @ModifyBy
                  WHERE VisitorId = @VisitorId AND IsActive = 1",
                connection);

            command.Parameters.Add("@VisitorId", SqlDbType.BigInt).Value = visitorId;
            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime2).Value = DateTime.UtcNow;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static void AddEditableParameters(SqlCommand command, VisitorLog visitorLog)
        {
            command.Parameters.Add("@VisitorName", SqlDbType.NVarChar, 100).Value = visitorLog.VisitorName;
            command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 15).Value =
                (object?)visitorLog.PhoneNumber ?? DBNull.Value;
            command.Parameters.Add("@VisitingFlatId", SqlDbType.BigInt).Value = visitorLog.VisitingFlatId;
            command.Parameters.Add("@Purpose", SqlDbType.NVarChar, 50).Value = visitorLog.Purpose;
            command.Parameters.Add("@VehicleNumber", SqlDbType.NVarChar, 20).Value =
                (object?)visitorLog.VehicleNumber ?? DBNull.Value;
            command.Parameters.Add("@ApprovalMethod", SqlDbType.NVarChar, 20).Value = visitorLog.ApprovalMethod;
            command.Parameters.Add("@EntryTime", SqlDbType.DateTime2).Value = visitorLog.EntryTime;
            command.Parameters.Add("@ExitTime", SqlDbType.DateTime2).Value =
                (object?)visitorLog.ExitTime ?? DBNull.Value;
            command.Parameters.Add("@LoggedByGuardId", SqlDbType.BigInt).Value =
                (object?)visitorLog.LoggedByGuardId ?? DBNull.Value;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)visitorLog.Description ?? DBNull.Value;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                (object?)visitorLog.TenantId ?? DBNull.Value;
        }

        private static VisitorLog MapVisitorLog(SqlDataReader reader)
        {
            return new VisitorLog
            {
                VisitorId = reader.GetInt64(reader.GetOrdinal("VisitorId")),
                VisitorName = reader.GetString(reader.GetOrdinal("VisitorName")),
                PhoneNumber = reader["PhoneNumber"] as string,
                VisitingFlatId = reader.GetInt64(reader.GetOrdinal("VisitingFlatId")),
                Purpose = reader.GetString(reader.GetOrdinal("Purpose")),
                VehicleNumber = reader["VehicleNumber"] as string,
                ApprovalMethod = reader.GetString(reader.GetOrdinal("ApprovalMethod")),
                EntryTime = reader.GetDateTime(reader.GetOrdinal("EntryTime")),
                ExitTime = reader["ExitTime"] == DBNull.Value
                    ? null : reader.GetDateTime(reader.GetOrdinal("ExitTime")),
                LoggedByGuardId = reader["LoggedByGuardId"] == DBNull.Value
                    ? null : reader.GetInt64(reader.GetOrdinal("LoggedByGuardId")),
                Description = reader["Description"] as string,
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value
                    ? null : reader.GetInt64(reader.GetOrdinal("CreatedBy")),
                ModifyOn = reader["ModifyOn"] == DBNull.Value
                    ? null : reader.GetDateTime(reader.GetOrdinal("ModifyOn")),
                ModifyBy = reader["ModifyBy"] == DBNull.Value
                    ? null : reader.GetInt64(reader.GetOrdinal("ModifyBy")),
                TenantId = reader["TenantId"] == DBNull.Value
                    ? null : reader.GetInt64(reader.GetOrdinal("TenantId"))
            };
        }
    }
}
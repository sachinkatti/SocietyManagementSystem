using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class SecurityGuardRepository : ISecurityGuardRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public SecurityGuardRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<SecurityGuard>> GetAllAsync()
        {
            var guards = new List<SecurityGuard>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.GetAllSecurityGuards", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                guards.Add(MapGuard(reader));
            }

            return guards;
        }

        public async Task<SecurityGuard?> GetByIdAsync(long guardId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.GetSecurityGuardById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@GuardId", SqlDbType.BigInt).Value = guardId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapGuard(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(SecurityGuard guard)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.InsertSecurityGuard", connection);

            command.CommandType = CommandType.StoredProcedure;
            AddGuardParameters(command, guard, false);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(SecurityGuard guard)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.UpdateSecurityGuard", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@GuardId", SqlDbType.BigInt).Value =
                guard.GuardId;

            AddGuardParameters(command, guard, true);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long guardId, long? modifyBy = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.DeleteSecurityGuard", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@GuardId", SqlDbType.BigInt).Value =
                guardId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        private static void AddGuardParameters(
            SqlCommand command,
            SecurityGuard guard,
            bool isUpdate)
        {
            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value =
                guard.UserId;

            command.Parameters.Add("@ShiftTiming", SqlDbType.NVarChar, 100).Value =
                guard.ShiftTiming;

            command.Parameters.Add("@AssignedGate", SqlDbType.NVarChar, 100).Value =
                (object?)guard.AssignedGate ?? DBNull.Value;

            command.Parameters.Add("@JoiningDate", SqlDbType.Date).Value =
                guard.JoiningDate.Date;

            string auditParameter = isUpdate ? "@ModifyBy" : "@CreatedBy";

            command.Parameters.Add(auditParameter, SqlDbType.BigInt).Value =
                (object?)(isUpdate ? guard.ModifyBy : guard.CreatedBy)
                ?? DBNull.Value;
        }

        private static SecurityGuard MapGuard(SqlDataReader reader)
        {
            return new SecurityGuard
            {
                GuardId = Convert.ToInt64(reader["GuardId"]),
                UserId = Convert.ToInt64(reader["UserId"]),
                FullName = reader["FullName"]?.ToString() ?? string.Empty,
                ShiftTiming = reader["ShiftTiming"]?.ToString() ?? string.Empty,
                AssignedGate = reader["AssignedGate"] == DBNull.Value
                    ? null
                    : reader["AssignedGate"].ToString(),
                JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value
                    ? null : Convert.ToInt64(reader["CreatedBy"]),
                ModifyOn = reader["ModifyOn"] == DBNull.Value
                    ? null : Convert.ToDateTime(reader["ModifyOn"]),
                ModifyBy = reader["ModifyBy"] == DBNull.Value
                    ? null : Convert.ToInt64(reader["ModifyBy"])
            };
        }
    }
}
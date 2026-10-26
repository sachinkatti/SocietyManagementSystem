
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
            using var command = new SqlCommand("GetAllSecurityGuards", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

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
            using var command = new SqlCommand("GetSecurityGuardById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@GuardId", SqlDbType.BigInt).Value = guardId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapGuard(reader) : null;
        }

        public async Task<long> CreateAsync(SecurityGuard guard)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("InsertSecurityGuard", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddGuardParameters(command, guard);

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)guard.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(SecurityGuard guard)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateSecurityGuard", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@GuardId", SqlDbType.BigInt).Value = guard.GuardId;
            AddGuardParameters(command, guard);

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)guard.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToBoolean(result);
        }

        public async Task<bool> DeleteAsync(long guardId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("DeleteSecurityGuard", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@GuardId", SqlDbType.BigInt).Value = guardId;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value = DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToBoolean(result);
        }

        private static void AddGuardParameters(SqlCommand command, SecurityGuard guard)
        {
            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value = guard.UserId;

            command.Parameters.Add("@ShiftTiming", SqlDbType.NVarChar, 50).Value =
                guard.ShiftTiming;

            command.Parameters.Add("@AssignedGate", SqlDbType.NVarChar, 50).Value =
                (object?)guard.AssignedGate ?? DBNull.Value;

            command.Parameters.Add("@JoiningDate", SqlDbType.Date).Value =
                guard.JoiningDate.Date;
        }

        private static SecurityGuard MapGuard(SqlDataReader reader)
        {
            return new SecurityGuard
            {
                GuardId = reader.GetInt64(reader.GetOrdinal("GuardId")),
                UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                ShiftTiming = reader.GetString(reader.GetOrdinal("ShiftTiming")),
                AssignedGate = reader.IsDBNull(reader.GetOrdinal("AssignedGate"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("AssignedGate")),
                JoiningDate = reader.GetDateTime(reader.GetOrdinal("JoiningDate")),
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
                    : reader.GetInt64(reader.GetOrdinal("ModifyBy"))
            };
        }
    }
}

using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public RoleRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            var roles = new List<Role>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllRoles", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add(MapRole(reader));
            }

            return roles;
        }

        public async Task<Role?> GetByIdAsync(long roleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetRoleById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@RoleId", SqlDbType.BigInt).Value = roleId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapRole(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Role role)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("InsertRole", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = role.TenantId;
            command.Parameters.Add("@RoleName", SqlDbType.NVarChar, 100).Value = role.RoleName;
            command.Parameters.Add("@RoleCode", SqlDbType.NVarChar, 50).Value = role.RoleCode;
            command.Parameters.Add("@RoleDescription", SqlDbType.NVarChar, 500).Value =
                (object?)role.RoleDescription ?? DBNull.Value;
            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)role.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException("The role was not created.");
            }

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Role role)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateRole", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@RoleId", SqlDbType.BigInt).Value = role.RoleId;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = role.TenantId;
            command.Parameters.Add("@RoleName", SqlDbType.NVarChar, 100).Value = role.RoleName;
            command.Parameters.Add("@RoleCode", SqlDbType.NVarChar, 50).Value = role.RoleCode;
            command.Parameters.Add("@RoleDescription", SqlDbType.NVarChar, 500).Value =
                (object?)role.RoleDescription ?? DBNull.Value;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = role.IsActive;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)role.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long roleId, long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("DeleteRole", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@RoleId", SqlDbType.BigInt).Value = roleId;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        private static Role MapRole(SqlDataReader reader)
        {
            return new Role
            {
                RoleId = reader.GetInt64(reader.GetOrdinal("RoleId")),
                TenantId = reader.GetInt64(reader.GetOrdinal("TenantId")),
                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                RoleCode = reader.GetString(reader.GetOrdinal("RoleCode")),
                RoleDescription = reader["RoleDescription"] == DBNull.Value
                    ? null
                    : reader.GetString(reader.GetOrdinal("RoleDescription")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedOn = reader["CreatedOn"] == DBNull.Value
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value
                    ? null
                    : reader.GetInt64(reader.GetOrdinal("CreatedBy")),
                ModifyOn = reader["ModifyOn"] == DBNull.Value
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("ModifyOn")),
                ModifyBy = reader["ModifyBy"] == DBNull.Value
                    ? null
                    : reader.GetInt64(reader.GetOrdinal("ModifyBy"))
            };
        }
    }
}
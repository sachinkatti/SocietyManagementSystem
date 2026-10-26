using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public RolePermissionRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<RolePermission>> GetAllAsync()
        {
            var mappings = new List<RolePermission>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllRolePermissions", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                mappings.Add(MapRolePermission(reader));
            }

            return mappings;
        }

        public async Task<RolePermission?> GetByIdAsync(long rolePermissionId)
        {
            var mappings = await GetAllAsync();

            return mappings.FirstOrDefault(
                item => item.RolePermissionId == rolePermissionId);
        }

        public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(long roleId)
        {
            var mappings = new List<RolePermission>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetPermissionsByRoleId", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@RoleId", SqlDbType.BigInt).Value = roleId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                mappings.Add(MapRolePermission(reader));
            }

            return mappings;
        }

        public async Task<long> AssignAsync(RolePermission rolePermission)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("AssignPermissionToRole", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@RoleId", SqlDbType.BigInt).Value =
                rolePermission.RoleId;

            command.Parameters.Add("@PermissionId", SqlDbType.BigInt).Value =
                rolePermission.PermissionId;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)rolePermission.Description ?? DBNull.Value;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)rolePermission.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "The permission could not be assigned to the role.");
            }

            return Convert.ToInt64(result);
        }

        public async Task<bool> RemoveAsync(
            long rolePermissionId,
            long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("RemovePermissionFromRole", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@RolePermissionId", SqlDbType.BigInt).Value =
                rolePermissionId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        private static RolePermission MapRolePermission(SqlDataReader reader)
        {
            var mapping = new RolePermission
            {
                RolePermissionId = reader.GetInt64(
                    reader.GetOrdinal("RolePermissionId")),

                RoleId = reader.GetInt64(
                    reader.GetOrdinal("RoleId")),

                PermissionId = reader.GetInt64(
                    reader.GetOrdinal("PermissionId")),

                Description = reader["Description"] == DBNull.Value
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive"))
            };

            if (HasColumn(reader, "RoleName") &&
                reader["RoleName"] != DBNull.Value)
            {
                mapping.RoleName = reader.GetString(
                    reader.GetOrdinal("RoleName"));
            }

            if (HasColumn(reader, "PermissionName") &&
                reader["PermissionName"] != DBNull.Value)
            {
                mapping.PermissionName = reader.GetString(
                    reader.GetOrdinal("PermissionName"));
            }

            if (HasColumn(reader, "PermissionCode") &&
                reader["PermissionCode"] != DBNull.Value)
            {
                mapping.PermissionCode = reader.GetString(
                    reader.GetOrdinal("PermissionCode"));
            }

            if (HasColumn(reader, "CreatedOn") &&
                reader["CreatedOn"] != DBNull.Value)
            {
                mapping.CreatedOn = reader.GetDateTime(
                    reader.GetOrdinal("CreatedOn"));
            }

            if (HasColumn(reader, "CreatedBy") &&
                reader["CreatedBy"] != DBNull.Value)
            {
                mapping.CreatedBy = reader.GetInt64(
                    reader.GetOrdinal("CreatedBy"));
            }

            if (HasColumn(reader, "ModifyOn") &&
                reader["ModifyOn"] != DBNull.Value)
            {
                mapping.ModifyOn = reader.GetDateTime(
                    reader.GetOrdinal("ModifyOn"));
            }

            if (HasColumn(reader, "ModifyBy") &&
                reader["ModifyBy"] != DBNull.Value)
            {
                mapping.ModifyBy = reader.GetInt64(
                    reader.GetOrdinal("ModifyBy"));
            }

            return mapping;
        }

        private static bool HasColumn(SqlDataReader reader, string columnName)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(
                    reader.GetName(i),
                    columnName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

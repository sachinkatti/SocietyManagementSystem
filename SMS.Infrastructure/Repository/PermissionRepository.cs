
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public PermissionRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            var permissions = new List<Permission>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllPermissions", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                permissions.Add(MapPermission(reader));
            }

            return permissions;
        }

        public async Task<Permission?> GetByIdAsync(long permissionId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetPermissionById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@PermissionId", SqlDbType.BigInt).Value = permissionId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapPermission(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Permission permission)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("InsertPermission", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PermissionName", SqlDbType.NVarChar, 100).Value =
                permission.PermissionName;

            command.Parameters.Add("@PermissionCode", SqlDbType.NVarChar, 100).Value =
                permission.PermissionCode;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)permission.Description ?? DBNull.Value;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)permission.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException("The permission was not created.");
            }

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Permission permission)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdatePermission", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PermissionId", SqlDbType.BigInt).Value =
                permission.PermissionId;

            command.Parameters.Add("@PermissionName", SqlDbType.NVarChar, 100).Value =
                permission.PermissionName;

            command.Parameters.Add("@PermissionCode", SqlDbType.NVarChar, 100).Value =
                permission.PermissionCode;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)permission.Description ?? DBNull.Value;

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
                permission.IsActive;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)permission.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long permissionId, long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("DeletePermission", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PermissionId", SqlDbType.BigInt).Value =
                permissionId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        private static Permission MapPermission(SqlDataReader reader)
        {
            return new Permission
            {
                PermissionId = reader.GetInt64(reader.GetOrdinal("PermissionId")),
                PermissionName = reader.GetString(reader.GetOrdinal("PermissionName")),
                PermissionCode = reader.GetString(reader.GetOrdinal("PermissionCode")),
                Description = reader["Description"] == DBNull.Value
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
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

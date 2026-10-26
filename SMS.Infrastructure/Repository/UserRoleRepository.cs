
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public UserRoleRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            var userRoles = new List<UserRole>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllUserRoles", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                userRoles.Add(MapUserRole(reader));
            }

            return userRoles;
        }

        public async Task<UserRole?> GetByIdAsync(long userRoleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetUserRoleById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@UserRoleId", SqlDbType.BigInt).Value = userRoleId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapUserRole(reader);
            }

            return null;
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(long userId)
        {
            var userRoles = new List<UserRole>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetUserRolesByUserId", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                userRoles.Add(MapUserRole(reader));
            }

            return userRoles;
        }

        public async Task<long> AssignAsync(UserRole userRole)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("AssignRoleToUser", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value =
                userRole.UserId;

            command.Parameters.Add("@RoleId", SqlDbType.BigInt).Value =
                userRole.RoleId;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)userRole.Description ?? DBNull.Value;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)userRole.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "The role could not be assigned to the user.");
            }

            return Convert.ToInt64(result);
        }

        public async Task<bool> RemoveAsync(long userRoleId, long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("RemoveRoleFromUser", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@UserRoleId", SqlDbType.BigInt).Value =
                userRoleId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        private static UserRole MapUserRole(SqlDataReader reader)
        {
            return new UserRole
            {
                UserRoleId = reader.GetInt64(reader.GetOrdinal("UserRoleId")),
                UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                UserName = reader["UserName"] == DBNull.Value
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("UserName")),
                RoleId = reader.GetInt64(reader.GetOrdinal("RoleId")),
                RoleName = reader["RoleName"] == DBNull.Value
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("RoleName")),
                RoleCode = reader["RoleCode"] == DBNull.Value
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("RoleCode")),
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

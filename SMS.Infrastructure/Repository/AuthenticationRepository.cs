
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public AuthenticationRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetUserByEmail", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value =
                email.Trim();

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new User
            {
                UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                TenantId = reader.GetInt64(reader.GetOrdinal("TenantId")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName"))
                    ? null : reader.GetString(reader.GetOrdinal("MiddleName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber"))
                    ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                UserDescription = reader.IsDBNull(reader.GetOrdinal("UserDescription"))
                    ? null : reader.GetString(reader.GetOrdinal("UserDescription")),
                PasswordHash = reader.IsDBNull(reader.GetOrdinal("PasswordHash"))
                    ? null : reader.GetString(reader.GetOrdinal("PasswordHash")),
                FailedLoginCount = reader.GetInt32(reader.GetOrdinal("FailedLoginCount")),
                LockoutEndUtc = reader.IsDBNull(reader.GetOrdinal("LockoutEndUtc"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("LockoutEndUtc")),
                LastLoginOn = reader.IsDBNull(reader.GetOrdinal("LastLoginOn"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("LastLoginOn")),
                MustChangePassword = reader.GetBoolean(reader.GetOrdinal("MustChangePassword")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedOn = reader.IsDBNull(reader.GetOrdinal("CreatedOn"))
                    ? default : reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null : reader.GetInt64(reader.GetOrdinal("CreatedBy")),
                ModifyOn = reader.IsDBNull(reader.GetOrdinal("ModifyOn"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("ModifyOn")),
                ModifyBy = reader.IsDBNull(reader.GetOrdinal("ModifyBy"))
                    ? null : reader.GetInt64(reader.GetOrdinal("ModifyBy"))
            };
        }

        public async Task<List<string>> GetUserRolesAsync(long userId)
        {
            var roles = new List<string>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetUserRoles", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                roles.Add(reader.GetString(0));

            return roles;
        }

        public async Task<List<string>> GetUserPermissionsAsync(long userId)
        {
            var permissions = new List<string>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetUserPermissions", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                permissions.Add(reader.GetString(0));

            return permissions;
        }

        public async Task<bool> UpdateFailedLoginAsync(
            long userId,
            int failedLoginCount,
            DateTime? lockoutEndUtc)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateFailedLogin", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;
            command.Parameters.Add("@FailedLoginCount", SqlDbType.Int).Value =
                failedLoginCount;
            command.Parameters.Add("@LockoutEndUtc", SqlDbType.DateTime2).Value =
                (object?)lockoutEndUtc ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        public async Task UpdateSuccessfulLoginAsync(long userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateSuccessfulLogin", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> SetMissingPasswordHashAsync(
            long userId,
            string passwordHash)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.SetMissingUserPasswordHash", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, -1).Value =
                passwordHash;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) == 1;
        }

        public async Task<bool> ChangePasswordAsync(
            long userId,
            string passwordHash)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.ChangeUserPassword", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value =
                passwordHash;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) == 1;
        }
    }
}
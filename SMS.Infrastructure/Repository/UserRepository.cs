
using Microsoft.Data.SqlClient;
using System.Data;
using SMS.Domain;

namespace SMS.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnectionFactory _factory;

        public UserRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = new List<User>();

            using var con = _factory.CreateConnection();
            using var cmd = new SqlCommand("dbo.GetAllUsers", con);

            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(MapUser(reader));
            }

            return users;
        }

        public async Task<User?> GetByIdAsync(long userId)
        {
            using var con = _factory.CreateConnection();
            using var cmd = new SqlCommand("dbo.GetByUserId", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapUser(reader) : null;
        }

        public async Task<long> CreateAsync(User user)
        {
            using var con = _factory.CreateConnection();
            using var cmd = new SqlCommand("dbo.InsertUser", con);

            cmd.CommandType = CommandType.StoredProcedure;
            AddUserParameters(cmd, user);

            cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)user.CreatedBy ?? DBNull.Value;

            await con.OpenAsync();

            var result = await cmd.ExecuteScalarAsync();

            return result == null || result == DBNull.Value
                ? 0
                : Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(User user)
        {
            using var con = _factory.CreateConnection();
            using var cmd = new SqlCommand("dbo.UpdateUser", con);

            cmd.CommandType = CommandType.StoredProcedure;
            AddUserParameters(cmd, user);

            cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value =
                user.UserId;

            cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
                user.IsActive;

            cmd.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)user.ModifyBy ?? DBNull.Value;

            await con.OpenAsync();

            var result = await cmd.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long userId)
        {
            using var con = _factory.CreateConnection();
            using var cmd = new SqlCommand("dbo.DeleteUser", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value =
                userId;

            // The existing interface doesn't provide ModifyBy.
            cmd.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                DBNull.Value;

            await con.OpenAsync();

            var result = await cmd.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        public async Task<IEnumerable<User>> GetActiveUsersForDropdownAsync()
        {
            var users = new List<User>();

            using var con = _factory.CreateConnection();
            using var cmd = new SqlCommand(
                "dbo.GetActiveUsersForDropdown", con);

            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(MapUser(reader));
            }

            return users;
        }

        private static void AddUserParameters(SqlCommand cmd, User user)
        {
            cmd.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                user.TenantId;

            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 300).Value =
                user.FirstName;

            cmd.Parameters.Add("@MiddleName", SqlDbType.NVarChar, 300).Value =
                (object?)user.MiddleName ?? DBNull.Value;

            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 300).Value =
                user.LastName;

            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 300).Value =
                user.Email;

            cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 30).Value =
                (object?)user.PhoneNumber ?? DBNull.Value;

            cmd.Parameters.Add("@UserDescription", SqlDbType.NVarChar, 1000).Value =
                (object?)user.UserDescription ?? DBNull.Value;
        }

        private static User MapUser(SqlDataReader reader)
        {
            return new User
            {
                UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                TenantId = reader.GetInt64(reader.GetOrdinal("TenantId")),

                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),

                MiddleName = GetNullableString(reader, "MiddleName"),

                LastName = reader.GetString(reader.GetOrdinal("LastName")),

                Email = reader.GetString(reader.GetOrdinal("Email")),

                PhoneNumber = GetNullableString(reader, "PhoneNumber"),

                UserDescription = GetNullableString(reader, "UserDescription"),

                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),

                CreatedOn = GetNullableDateTime(reader, "CreatedOn")
                    ?? DateTime.MinValue,

                CreatedBy = GetNullableLong(reader, "CreatedBy"),

                ModifyOn = GetNullableDateTime(reader, "ModifyOn"),

                ModifyBy = GetNullableLong(reader, "ModifyBy"),

                PasswordHash = GetNullableString(reader, "PasswordHash"),

                FailedLoginCount = GetInt32OrDefault(reader, "FailedLoginCount"),

                LockoutEndUtc = GetNullableDateTime(reader, "LockoutEndUtc"),

                LastLoginOn = GetNullableDateTime(reader, "LastLoginOn"),

                MustChangePassword = GetBooleanOrDefault(reader, "MustChangePassword")
            };
        }

        private static int GetOrdinalIfExists(
            SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(
                    reader.GetName(i),
                    columnName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private static string? GetNullableString(
            SqlDataReader reader, string columnName)
        {
            int ordinal = GetOrdinalIfExists(reader, columnName);

            return ordinal < 0 || reader.IsDBNull(ordinal)
                ? null
                : Convert.ToString(reader.GetValue(ordinal));
        }

        private static long? GetNullableLong(
            SqlDataReader reader, string columnName)
        {
            int ordinal = GetOrdinalIfExists(reader, columnName);

            return ordinal < 0 || reader.IsDBNull(ordinal)
                ? null
                : Convert.ToInt64(reader.GetValue(ordinal));
        }

        private static DateTime? GetNullableDateTime(
            SqlDataReader reader, string columnName)
        {
            int ordinal = GetOrdinalIfExists(reader, columnName);

            return ordinal < 0 || reader.IsDBNull(ordinal)
                ? null
                : Convert.ToDateTime(reader.GetValue(ordinal));
        }

        private static int GetInt32OrDefault(
            SqlDataReader reader, string columnName)
        {
            int ordinal = GetOrdinalIfExists(reader, columnName);

            return ordinal < 0 || reader.IsDBNull(ordinal)
                ? 0
                : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static bool GetBooleanOrDefault(
            SqlDataReader reader, string columnName)
        {
            int ordinal = GetOrdinalIfExists(reader, columnName);

            return ordinal >= 0 &&
                   !reader.IsDBNull(ordinal) &&
                   Convert.ToBoolean(reader.GetValue(ordinal));
        }
    }
}


using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public AdminRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Admin>> GetAllAsync()
        {
            var admins = new List<Admin>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.GetAllAdmins", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                admins.Add(MapAdmin(reader));
            }

            return admins;
        }

        public async Task<Admin?> GetByIdAsync(long adminId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.GetAdminById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@AdminId", SqlDbType.BigInt).Value = adminId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapAdmin(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Admin admin, long? createdBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.InsertAdmin", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value =
                admin.UserId;

            command.Parameters.Add("@Designation", SqlDbType.NVarChar, 200).Value =
                admin.Designation;

            command.Parameters.Add("@ContactNumber", SqlDbType.NVarChar, 30).Value =
                (object?)admin.ContactNumber ?? DBNull.Value;

            command.Parameters.Add("@DateAssigned", SqlDbType.DateTime2).Value =
                admin.DateAssigned;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)createdBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null && result != DBNull.Value
                ? Convert.ToInt64(result)
                : 0;
        }

        public async Task<bool> UpdateAsync(Admin admin, long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.UpdateAdmin", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@AdminId", SqlDbType.BigInt).Value =
                admin.AdminId;

            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value =
                admin.UserId;

            command.Parameters.Add("@Designation", SqlDbType.NVarChar, 200).Value =
                admin.Designation;

            command.Parameters.Add("@ContactNumber", SqlDbType.NVarChar, 30).Value =
                (object?)admin.ContactNumber ?? DBNull.Value;

            command.Parameters.Add("@DateAssigned", SqlDbType.DateTime2).Value =
                admin.DateAssigned;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long adminId, long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.DeleteAdmin", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@AdminId", SqlDbType.BigInt).Value =
                adminId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }


        public async Task<List<User>> GetAvailableUsersForAdminAsync()
        {
            var users = new List<User>();

            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "dbo.GetAvailableUsersForAdmin", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    UserId = Convert.ToInt64(reader["UserId"]),
                    TenantId = Convert.ToInt64(reader["TenantId"]),
                    FirstName = Convert.ToString(reader["FirstName"]) ?? string.Empty,

                    MiddleName = reader["MiddleName"] == DBNull.Value
                        ? null
                        : Convert.ToString(reader["MiddleName"]),

                    LastName = Convert.ToString(reader["LastName"]) ?? string.Empty,
                    Email = Convert.ToString(reader["Email"]) ?? string.Empty,

                    PhoneNumber = reader["PhoneNumber"] == DBNull.Value
                        ? null
                        : Convert.ToString(reader["PhoneNumber"])
                });
            }

            return users;
        }

        private static Admin MapAdmin(SqlDataReader reader)
        {
            return new Admin
            {
                AdminId = Convert.ToInt64(reader["AdminId"]),
                UserId = Convert.ToInt64(reader["UserId"]),
                TenantId = Convert.ToInt64(reader["TenantId"]),

                FirstName = Convert.ToString(reader["FirstName"]) ?? string.Empty,

                MiddleName = reader["MiddleName"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["MiddleName"]),

                LastName = Convert.ToString(reader["LastName"]) ?? string.Empty,
                Email = Convert.ToString(reader["Email"]) ?? string.Empty,

                PhoneNumber = reader["PhoneNumber"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["PhoneNumber"]),

                Designation = Convert.ToString(reader["Designation"]) ?? string.Empty,

                ContactNumber = reader["ContactNumber"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["ContactNumber"]),

                DateAssigned = Convert.ToDateTime(reader["DateAssigned"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),

                CreatedBy = reader["CreatedBy"] == DBNull.Value
                    ? null
                    : Convert.ToInt64(reader["CreatedBy"]),

                ModifyOn = reader["ModifyOn"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["ModifyOn"]),

                ModifyBy = reader["ModifyBy"] == DBNull.Value
                    ? null
                    : Convert.ToInt64(reader["ModifyBy"])
            };
        }
    }
}
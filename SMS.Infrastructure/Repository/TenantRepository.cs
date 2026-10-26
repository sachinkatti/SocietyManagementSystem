using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class TenantRepository : ITenantRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TenantRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            var tenants = new List<Tenant>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand(
                "GetAllTenants", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tenants.Add(MapTenant(reader));
            }

            return tenants;
        }

        public async Task<Tenant?> GetByIdAsync(long tenantId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand(
                "GetByTenantId", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                tenantId;

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapTenant(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Tenant tenant)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand(
                "InsertTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            AddTenantParameters(command, tenant, false);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Tenant tenant)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand(
                "UpdateTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                tenant.TenantId;

            AddTenantParameters(command, tenant, true);

            var modifyByParameter = command.Parameters.Add(
                "@ModifyBy", SqlDbType.BigInt);

            modifyByParameter.Value = tenant.ModifyBy.HasValue
                ? tenant.ModifyBy.Value
                : DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long tenantId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand(
                "DeleteTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                tenantId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value = 1L;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        private static void AddTenantParameters(
            SqlCommand command,
            Tenant tenant,
            bool includeIsActive)
        {
            command.Parameters.Add("@TenantName", SqlDbType.NVarChar, 150)
                .Value = tenant.TenantName;

            command.Parameters.Add("@TenantCode", SqlDbType.NVarChar, 50)
                .Value = tenant.TenantCode;

            command.Parameters.Add("@TenantAddress", SqlDbType.NVarChar, 250)
                .Value = (object?)tenant.TenantAddress ?? DBNull.Value;

            command.Parameters.Add("@Email", SqlDbType.NVarChar, 256)
                .Value = (object?)tenant.Email ?? DBNull.Value;

            command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 15)
                .Value = (object?)tenant.PhoneNumber ?? DBNull.Value;

            command.Parameters.Add("@TenantDescription", SqlDbType.NVarChar, 500)
                .Value = (object?)tenant.TenantDescription ?? DBNull.Value;

            if (includeIsActive)
            {
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
                    tenant.IsActive;
            }
            else
            {
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = true;

                command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                    tenant.CreatedBy.HasValue
                        ? tenant.CreatedBy.Value
                        : DBNull.Value;
            }
        }

        private static Tenant MapTenant(SqlDataReader reader)
        {
            return new Tenant
            {
                TenantId = Convert.ToInt64(reader["TenantId"]),
                TenantName = Convert.ToString(reader["TenantName"]) ?? string.Empty,
                TenantCode = Convert.ToString(reader["TenantCode"]) ?? string.Empty,
                TenantAddress = reader["TenantAddress"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["TenantAddress"]),
                Email = reader["Email"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["Email"]),
                PhoneNumber = reader["PhoneNumber"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["PhoneNumber"]),
                TenantDescription = reader["TenantDescription"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["TenantDescription"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedOn = reader["CreatedOn"] == DBNull.Value
                    ? default
                    : Convert.ToDateTime(reader["CreatedOn"]),
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
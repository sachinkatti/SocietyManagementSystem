using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class DistrictRepository : IDistrictRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public DistrictRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<District>> GetAllAsync()
        {
            var districts = new List<District>();

            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "GetAllDistrict",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                districts.Add(MapDistrict(reader));
            }

            return districts;
        }

        public async Task<District?> GetByIdAsync(long districtId)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "GetByDistrictId",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@DistrictId",
                SqlDbType.BigInt).Value = districtId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapDistrict(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(District district)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "InsertDistrict",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@StateId",
                SqlDbType.BigInt).Value = district.StateId;

            command.Parameters.Add(
                "@DistrictName",
                SqlDbType.NVarChar, 100).Value =
                district.DistrictName;

            command.Parameters.Add(
                "@DistrictCode",
                SqlDbType.NVarChar, 20).Value =
                district.DistrictCode;

            command.Parameters.Add(
                "@Description",
                SqlDbType.NVarChar, 500).Value =
                (object?)district.Description ?? DBNull.Value;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit).Value =
                district.IsActive;

            command.Parameters.Add(
                "@CreatedBy",
                SqlDbType.BigInt).Value =
                (object?)district.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(District district)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "UpdateDistrict",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@DistrictId",
                SqlDbType.BigInt).Value =
                district.DistrictId;

            command.Parameters.Add(
                "@StateId",
                SqlDbType.BigInt).Value =
                district.StateId;

            command.Parameters.Add(
                "@DistrictName",
                SqlDbType.NVarChar, 100).Value =
                district.DistrictName;

            command.Parameters.Add(
                "@DistrictCode",
                SqlDbType.NVarChar, 20).Value =
                district.DistrictCode;

            command.Parameters.Add(
                "@Description",
                SqlDbType.NVarChar, 500).Value =
                (object?)district.Description ?? DBNull.Value;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit).Value =
                district.IsActive;

            command.Parameters.Add(
                "@ModifyBy",
                SqlDbType.BigInt).Value =
                (object?)district.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long districtId)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "DeleteDistrict",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@DistrictId",
                SqlDbType.BigInt).Value =
                districtId;

            command.Parameters.Add(
                "@ModifyBy",
                SqlDbType.BigInt).Value = 1;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        private static District MapDistrict(SqlDataReader reader)
        {
            return new District
            {
                DistrictId = Convert.ToInt64(
                    reader["DistrictId"]),

                StateId = Convert.ToInt64(
                    reader["StateId"]),

                DistrictName = reader["DistrictName"]?.ToString()
                    ?? string.Empty,

                DistrictCode = reader["DistrictCode"]?.ToString()
                    ?? string.Empty,

                Description = reader["Description"] == DBNull.Value
                    ? null
                    : reader["Description"].ToString(),

                IsActive = Convert.ToBoolean(
                    reader["IsActive"]),

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
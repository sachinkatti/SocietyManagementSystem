using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class CityRepository : ICityRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public CityRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            var cities = new List<City>();

            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "GetAllCity",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                cities.Add(MapCity(reader));
            }

            return cities;
        }

        public async Task<City?> GetByIdAsync(long cityId)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "GetByCityId",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@CityId",
                SqlDbType.BigInt).Value = cityId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapCity(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(City city)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "InsertCity",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@TalukId",
                SqlDbType.BigInt).Value =
                city.TalukId;

            command.Parameters.Add(
                "@CityName",
                SqlDbType.NVarChar, 100).Value =
                city.CityName;

            command.Parameters.Add(
                "@CityCode",
                SqlDbType.NVarChar, 20).Value =
                city.CityCode;

            command.Parameters.Add(
                "@PostalCode",
                SqlDbType.NVarChar, 20).Value =
                (object?)city.PostalCode ?? DBNull.Value;

            command.Parameters.Add(
                "@Description",
                SqlDbType.NVarChar, 500).Value =
                (object?)city.Description ?? DBNull.Value;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit).Value =
                city.IsActive;

            command.Parameters.Add(
                "@CreatedBy",
                SqlDbType.BigInt).Value =
                (object?)city.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(City city)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "UpdateCity",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@CityId",
                SqlDbType.BigInt).Value =
                city.CityId;

            command.Parameters.Add(
                "@TalukId",
                SqlDbType.BigInt).Value =
                city.TalukId;

            command.Parameters.Add(
                "@CityName",
                SqlDbType.NVarChar, 100).Value =
                city.CityName;

            command.Parameters.Add(
                "@CityCode",
                SqlDbType.NVarChar, 20).Value =
                city.CityCode;

            command.Parameters.Add(
                "@PostalCode",
                SqlDbType.NVarChar, 20).Value =
                (object?)city.PostalCode ?? DBNull.Value;

            command.Parameters.Add(
                "@Description",
                SqlDbType.NVarChar, 500).Value =
                (object?)city.Description ?? DBNull.Value;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit).Value =
                city.IsActive;

            command.Parameters.Add(
                "@ModifyBy",
                SqlDbType.BigInt).Value =
                (object?)city.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long cityId)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "DeleteCity",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@CityId",
                SqlDbType.BigInt).Value =
                cityId;

            command.Parameters.Add(
                "@ModifyBy",
                SqlDbType.BigInt).Value = 1;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        private static City MapCity(SqlDataReader reader)
        {
            return new City
            {
                CityId = Convert.ToInt64(
                    reader["CityId"]),

                TalukId = Convert.ToInt64(
                    reader["TalukId"]),

                CityName = reader["CityName"]?.ToString()
                    ?? string.Empty,

                CityCode = reader["CityCode"]?.ToString()
                    ?? string.Empty,

                PostalCode = reader["PostalCode"] == DBNull.Value
                    ? null
                    : reader["PostalCode"].ToString(),

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
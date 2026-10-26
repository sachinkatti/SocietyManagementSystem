using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public CountryRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            var countries = new List<Country>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllCountry", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                countries.Add(new Country
                {
                    CountryId = Convert.ToInt64(reader["CountryId"]),
                    RegionId = Convert.ToInt64(reader["RegionId"]),
                    CountryName = reader["CountryName"].ToString() ?? string.Empty,
                    CountryCode = reader["CountryCode"].ToString() ?? string.Empty,
                    Description = reader["Description"] == DBNull.Value
                        ? null
                        : reader["Description"].ToString(),
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
                });
            }

            return countries;
        }

        public async Task<Country?> GetByIdAsync(long countryId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetByCountryId", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@CountryId", SqlDbType.BigInt).Value = countryId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Country
                {
                    CountryId = Convert.ToInt64(reader["CountryId"]),
                    RegionId = Convert.ToInt64(reader["RegionId"]),
                    CountryName = reader["CountryName"].ToString() ?? string.Empty,
                    CountryCode = reader["CountryCode"].ToString() ?? string.Empty,
                    Description = reader["Description"] == DBNull.Value
                        ? null
                        : reader["Description"].ToString(),
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

            return null;
        }

        public async Task<long> CreateAsync(Country country)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("InsertCountry", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@RegionId", SqlDbType.BigInt).Value = country.RegionId;
            command.Parameters.Add("@CountryName", SqlDbType.NVarChar, 100).Value = country.CountryName;
            command.Parameters.Add("@CountryCode", SqlDbType.NVarChar, 10).Value = country.CountryCode;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)country.Description ?? DBNull.Value;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = country.IsActive;
            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)country.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Country country)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateCountry", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@CountryId", SqlDbType.BigInt).Value = country.CountryId;
            command.Parameters.Add("@RegionId", SqlDbType.BigInt).Value = country.RegionId;
            command.Parameters.Add("@CountryName", SqlDbType.NVarChar, 100).Value = country.CountryName;
            command.Parameters.Add("@CountryCode", SqlDbType.NVarChar, 10).Value = country.CountryCode;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)country.Description ?? DBNull.Value;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = country.IsActive;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)country.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long countryId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("DeleteCountry", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@CountryId", SqlDbType.BigInt).Value = countryId;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value = 1;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }
    }
}
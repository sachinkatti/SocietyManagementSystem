using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class RegionRepository : IRegionRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public RegionRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            var regions = new List<Region>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllRegion", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                regions.Add(new Region
                {
                    RegionId = Convert.ToInt64(reader["RegionId"]),
                    RegionName = reader["RegionName"]?.ToString() ?? string.Empty,
                    RegionCode = reader["RegionCode"]?.ToString() ?? string.Empty,
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

            return regions;
        }

        public async Task<Region?> GetByIdAsync(long regionId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetByRegionId", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@RegionId", SqlDbType.BigInt).Value = regionId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Region
            {
                RegionId = Convert.ToInt64(reader["RegionId"]),
                RegionName = reader["RegionName"]?.ToString() ?? string.Empty,
                RegionCode = reader["RegionCode"]?.ToString() ?? string.Empty,
                Description = reader["Description"] == DBNull.Value
                    ? null
                    : reader["Description"].ToString(),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                ModifyOn = reader["ModifyOn"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["ModifyOn"]),
                ModifyBy = reader["ModifyBy"] == DBNull.Value
                    ? null
                    : Convert.ToInt64(reader["ModifyBy"])
            };
        }

        public async Task<long> CreateAsync(Region region)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("InsertRegion", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@RegionName", SqlDbType.NVarChar, 100).Value =
                region.RegionName;

            command.Parameters.Add("@RegionCode", SqlDbType.NVarChar, 20).Value =
                region.RegionCode;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)region.Description ?? DBNull.Value;

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
                region.IsActive;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)region.CreatedBy ?? DBNull.Value;

            command.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value =
                region.CreatedOn;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)region.ModifyBy ?? DBNull.Value;

            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime).Value =
                region.ModifyOn ?? (object)DBNull.Value;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            return 0;
        }

        public async Task<bool> UpdateAsync(Region region)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateRegion", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@RegionId", SqlDbType.BigInt).Value =
                region.RegionId;

            command.Parameters.Add("@RegionName", SqlDbType.NVarChar, 100).Value =
                region.RegionName;

            command.Parameters.Add("@RegionCode", SqlDbType.NVarChar, 20).Value =
                region.RegionCode;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)region.Description ?? DBNull.Value;

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
                region.IsActive;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)region.ModifyBy ?? DBNull.Value;

            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime).Value =
                region.ModifyOn ?? (object)DBNull.Value;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(long regionId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("DeleteRegion", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@RegionId", SqlDbType.BigInt).Value = regionId;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            return true;
        }
    }
}
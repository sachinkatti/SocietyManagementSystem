
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class WingRepository : IWingRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public WingRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private Wing MapWing(SqlDataReader reader)
        {
            return new Wing
            {
                WingId = reader.GetInt64(reader.GetOrdinal("WingId")),
                TenantId = reader.GetInt64(reader.GetOrdinal("TenantId")),
                WingName = reader.GetString(reader.GetOrdinal("WingName")),
                TotalFloors = reader.GetInt32(reader.GetOrdinal("TotalFloors")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null
                    : reader.GetInt64(reader.GetOrdinal("CreatedBy")),
                ModifyOn = reader.IsDBNull(reader.GetOrdinal("ModifyOn"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("ModifyOn")),
                ModifyBy = reader.IsDBNull(reader.GetOrdinal("ModifyBy"))
                    ? null
                    : reader.GetInt64(reader.GetOrdinal("ModifyBy"))
            };
        }

        public async Task<IEnumerable<Wing>> GetAllAsync()
        {
            var wings = new List<Wing>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllWings", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                wings.Add(MapWing(reader));
            }

            return wings;
        }

        public async Task<Wing?> GetByIdAsync(long wingId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetWingById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@WingId", SqlDbType.BigInt).Value = wingId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapWing(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Wing wing)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("InsertWing", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = wing.TenantId;
            command.Parameters.Add("@WingName", SqlDbType.NVarChar, 100).Value = wing.WingName;
            command.Parameters.Add("@TotalFloors", SqlDbType.Int).Value = wing.TotalFloors;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value =
                (object?)wing.Description ?? DBNull.Value;
            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)wing.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Wing wing)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateWing", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@WingId", SqlDbType.BigInt).Value = wing.WingId;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = wing.TenantId;
            command.Parameters.Add("@WingName", SqlDbType.NVarChar, 100).Value = wing.WingName;
            command.Parameters.Add("@TotalFloors", SqlDbType.Int).Value = wing.TotalFloors;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value =
                (object?)wing.Description ?? DBNull.Value;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)wing.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long wingId)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand("dbo.DeleteWing", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@WingId", SqlDbType.BigInt).Value = wingId;

            // NULL means the current user ID is not being supplied.
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }
    }
}

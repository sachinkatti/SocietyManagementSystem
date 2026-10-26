
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class FlatRepository : IFlatRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public FlatRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private static Flat MapFlat(SqlDataReader reader)
        {
            return new Flat
            {
                FlatId = reader.GetInt64(reader.GetOrdinal("FlatId")),
                WingId = reader.GetInt64(reader.GetOrdinal("WingId")),
                FlatNumber = reader.GetString(reader.GetOrdinal("FlatNumber")),
                FloorNumber = reader.GetInt32(reader.GetOrdinal("FloorNumber")),
                SquareFeet = reader.GetDecimal(reader.GetOrdinal("SquareFeet")),
                OccupancyStatus = reader.GetString(reader.GetOrdinal("OccupancyStatus")),
                ParkingSlotNumber = reader.IsDBNull(reader.GetOrdinal("ParkingSlotNumber"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ParkingSlotNumber")),
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

        public async Task<IEnumerable<Flat>> GetAllAsync()
        {
            var flats = new List<Flat>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllFlats", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                flats.Add(MapFlat(reader));
            }

            return flats;
        }

        public async Task<Flat?> GetByIdAsync(long flatId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetFlatById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value = flatId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapFlat(reader) : null;
        }

        public async Task<long> CreateAsync(Flat flat)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("InsertFlat", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@WingId", SqlDbType.BigInt).Value = flat.WingId;
            command.Parameters.Add("@FlatNumber", SqlDbType.NVarChar, 10).Value = flat.FlatNumber;
            command.Parameters.Add("@FloorNumber", SqlDbType.Int).Value = flat.FloorNumber;

            var squareFeetParameter = command.Parameters.Add("@SquareFeet", SqlDbType.Decimal);
            squareFeetParameter.Precision = 18;
            squareFeetParameter.Scale = 2;
            squareFeetParameter.Value = flat.SquareFeet;

            command.Parameters.Add("@OccupancyStatus", SqlDbType.NVarChar, 20).Value = flat.OccupancyStatus;
            command.Parameters.Add("@ParkingSlotNumber", SqlDbType.NVarChar, 20).Value =
                (object?)flat.ParkingSlotNumber ?? DBNull.Value;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)flat.Description ?? DBNull.Value;
            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)flat.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Flat flat)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateFlat", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value = flat.FlatId;
            command.Parameters.Add("@WingId", SqlDbType.BigInt).Value = flat.WingId;
            command.Parameters.Add("@FlatNumber", SqlDbType.NVarChar, 10).Value = flat.FlatNumber;
            command.Parameters.Add("@FloorNumber", SqlDbType.Int).Value = flat.FloorNumber;

            var squareFeetParameter = command.Parameters.Add("@SquareFeet", SqlDbType.Decimal);
            squareFeetParameter.Precision = 18;
            squareFeetParameter.Scale = 2;
            squareFeetParameter.Value = flat.SquareFeet;

            command.Parameters.Add("@OccupancyStatus", SqlDbType.NVarChar, 20).Value = flat.OccupancyStatus;
            command.Parameters.Add("@ParkingSlotNumber", SqlDbType.NVarChar, 20).Value =
                (object?)flat.ParkingSlotNumber ?? DBNull.Value;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)flat.Description ?? DBNull.Value;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)flat.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long flatId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("DeleteFlat", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value = flatId;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }
    }
}

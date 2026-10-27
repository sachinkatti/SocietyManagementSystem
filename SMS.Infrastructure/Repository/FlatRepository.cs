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

        public async Task<IEnumerable<Flat>> GetAllAsync()
        {
            var flats = new List<Flat>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.GetAllFlats", connection);

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
            using var command = new SqlCommand("dbo.GetFlatById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value = flatId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapFlat(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Flat flat)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.InsertFlat", connection);

            command.CommandType = CommandType.StoredProcedure;

            AddFlatParameters(command, flat, false);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Flat flat)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.UpdateFlat", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value = flat.FlatId;

            AddFlatParameters(command, flat, true);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long flatId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.DeleteFlat", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value =
                flatId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        private static void AddFlatParameters(
            SqlCommand command,
            Flat flat,
            bool isUpdate)
        {
            command.Parameters.Add("@WingId", SqlDbType.BigInt).Value = flat.WingId;
            command.Parameters.Add("@FlatNumber", SqlDbType.NVarChar, 50).Value = flat.FlatNumber;
            command.Parameters.Add("@FloorNumber", SqlDbType.Int).Value = flat.FloorNumber;

            var squareFeet = command.Parameters.Add("@SquareFeet", SqlDbType.Decimal);
            squareFeet.Precision = 18;
            squareFeet.Scale = 2;
            squareFeet.Value = flat.SquareFeet;

            command.Parameters.Add("@OccupancyStatus", SqlDbType.NVarChar, 50).Value = flat.OccupancyStatus;
            command.Parameters.Add("@ParkingSlotNumber", SqlDbType.NVarChar, 50).Value =
                (object?)flat.ParkingSlotNumber ?? DBNull.Value;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value =
                (object?)flat.Description ?? DBNull.Value;

            if (isUpdate)
            {
                command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                    DBNull.Value;
            }
            else
            {
                command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                    DBNull.Value;
            }
        }

        private static Flat MapFlat(SqlDataReader reader)
        {
            return new Flat
            {
                FlatId = Convert.ToInt64(reader["FlatId"]),
                WingId = Convert.ToInt64(reader["WingId"]),
                WingName = reader["WingName"].ToString() ?? string.Empty,
                FlatNumber = reader["FlatNumber"].ToString() ?? string.Empty,
                FloorNumber = Convert.ToInt32(reader["FloorNumber"]),
                SquareFeet = Convert.ToDecimal(reader["SquareFeet"]),
                OccupancyStatus =
                    reader["OccupancyStatus"].ToString() ?? string.Empty,
                ParkingSlotNumber = reader["ParkingSlotNumber"] == DBNull.Value
                    ? null
                    : reader["ParkingSlotNumber"].ToString(),
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
    }
}
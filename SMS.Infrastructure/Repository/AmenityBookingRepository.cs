
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class AmenityBookingRepository : IAmenityBookingRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public AmenityBookingRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<AmenityBooking>> GetAllAsync()
        {
            var bookings = new List<AmenityBooking>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT BookingId, AmenityId, FlatId, SlotDate, SlotTime,
                         DepositPaid, Status, Description, IsActive,
                         CreatedOn, CreatedBy, ModifyOn, ModifyBy, TenantId
                  FROM AmenityBookings
                  WHERE IsActive = 1
                  ORDER BY BookingId DESC",
                connection);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                bookings.Add(MapBooking(reader));
            }

            return bookings;
        }

        public async Task<AmenityBooking?> GetByIdAsync(long bookingId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT BookingId, AmenityId, FlatId, SlotDate, SlotTime,
                         DepositPaid, Status, Description, IsActive,
                         CreatedOn, CreatedBy, ModifyOn, ModifyBy, TenantId
                  FROM AmenityBookings
                  WHERE BookingId = @BookingId AND IsActive = 1",
                connection);

            command.Parameters.Add("@BookingId", SqlDbType.BigInt).Value = bookingId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapBooking(reader) : null;
        }

        public async Task<long> CreateAsync(AmenityBooking booking)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"INSERT INTO AmenityBookings
                    (AmenityId, FlatId, SlotDate, SlotTime, DepositPaid,
                     Status, Description, IsActive, CreatedOn, CreatedBy, TenantId)
                  VALUES
                    (@AmenityId, @FlatId, @SlotDate, @SlotTime, @DepositPaid,
                     @Status, @Description, @IsActive, @CreatedOn, @CreatedBy, @TenantId);
                  SELECT CAST(SCOPE_IDENTITY() AS BIGINT);",
                connection);

            AddBookingParameters(command, booking);

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = true;
            command.Parameters.Add("@CreatedOn", SqlDbType.DateTime2).Value = DateTime.UtcNow;
            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)booking.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            return Convert.ToInt64(await command.ExecuteScalarAsync());
        }

        public async Task<bool> UpdateAsync(AmenityBooking booking)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"UPDATE AmenityBookings
                  SET AmenityId = @AmenityId,
                      FlatId = @FlatId,
                      SlotDate = @SlotDate,
                      SlotTime = @SlotTime,
                      DepositPaid = @DepositPaid,
                      Status = @Status,
                      Description = @Description,
                      TenantId = @TenantId,
                      ModifyOn = @ModifyOn,
                      ModifyBy = @ModifyBy
                  WHERE BookingId = @BookingId AND IsActive = 1",
                connection);

            AddBookingParameters(command, booking);

            command.Parameters.Add("@BookingId", SqlDbType.BigInt).Value = booking.BookingId;
            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime2).Value = DateTime.UtcNow;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)booking.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long bookingId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"UPDATE AmenityBookings
                  SET IsActive = 0, ModifyOn = @ModifyOn
                  WHERE BookingId = @BookingId AND IsActive = 1",
                connection);

            command.Parameters.Add("@BookingId", SqlDbType.BigInt).Value = bookingId;
            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime2).Value = DateTime.UtcNow;

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static void AddBookingParameters(SqlCommand command, AmenityBooking booking)
        {
            command.Parameters.Add("@AmenityId", SqlDbType.BigInt).Value = booking.AmenityId;
            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value = booking.FlatId;
            command.Parameters.Add("@SlotDate", SqlDbType.Date).Value = booking.SlotDate.Date;
            command.Parameters.Add("@SlotTime", SqlDbType.NVarChar, 20).Value = booking.SlotTime;

            var depositParameter = command.Parameters.Add("@DepositPaid", SqlDbType.Decimal);
            depositParameter.Precision = 18;
            depositParameter.Scale = 2;
            depositParameter.Value = booking.DepositPaid;

            command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = booking.Status;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)booking.Description ?? DBNull.Value;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = booking.TenantId;
        }

        private static AmenityBooking MapBooking(SqlDataReader reader)
        {
            return new AmenityBooking
            {
                BookingId = reader.GetInt64(reader.GetOrdinal("BookingId")),
                AmenityId = reader.GetInt64(reader.GetOrdinal("AmenityId")),
                FlatId = reader.GetInt64(reader.GetOrdinal("FlatId")),
                SlotDate = reader.GetDateTime(reader.GetOrdinal("SlotDate")),
                SlotTime = reader.GetString(reader.GetOrdinal("SlotTime")),
                DepositPaid = reader.GetDecimal(reader.GetOrdinal("DepositPaid")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
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
                    : reader.GetInt64(reader.GetOrdinal("ModifyBy")),
                TenantId = reader.GetInt64(reader.GetOrdinal("TenantId"))
            };
        }
    }
}

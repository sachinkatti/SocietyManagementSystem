
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
                "GetAllAmenityBookings", connection);

            command.CommandType = CommandType.StoredProcedure;

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
                "GetAmenityBookingById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@BookingId", SqlDbType.BigInt).Value =
                bookingId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapBooking(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(AmenityBooking booking)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "InsertAmenityBooking", connection);

            command.CommandType = CommandType.StoredProcedure;

            AddBookingParameters(command, booking);

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)booking.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(AmenityBooking booking)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UpdateAmenityBooking", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@BookingId", SqlDbType.BigInt).Value =
                booking.BookingId;

            AddBookingParameters(command, booking);

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)booking.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(
            long bookingId, long? modifyBy = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "DeleteAmenityBooking", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@BookingId", SqlDbType.BigInt).Value =
                bookingId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<IEnumerable<AmenityBookingAmenityOption>>
            GetAmenitiesByTenantAsync(long tenantId)
        {
            var amenities = new List<AmenityBookingAmenityOption>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "GetBookingAmenitiesByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                tenantId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                amenities.Add(new AmenityBookingAmenityOption
                {
                    AmenityId = Convert.ToInt64(reader["AmenityId"]),
                    AmenityName = Convert.ToString(
                        reader["AmenityName"]) ?? string.Empty
                });
            }

            return amenities;
        }

        public async Task<IEnumerable<AmenityBookingFlatOption>>
            GetFlatsByTenantAsync(long tenantId)
        {
            var flats = new List<AmenityBookingFlatOption>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "GetBookingFlatsByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                tenantId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                flats.Add(new AmenityBookingFlatOption
                {
                    FlatId = Convert.ToInt64(reader["FlatId"]),
                    FlatNumber = Convert.ToString(
                        reader["FlatNumber"]) ?? string.Empty,
                    WingId = Convert.ToInt64(reader["WingId"]),
                    WingName = Convert.ToString(
                        reader["WingName"]) ?? string.Empty
                });
            }

            return flats;
        }

        private static void AddBookingParameters(
            SqlCommand command, AmenityBooking booking)
        {
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                booking.TenantId;

            command.Parameters.Add("@AmenityId", SqlDbType.BigInt).Value =
                booking.AmenityId;

            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value =
                booking.FlatId;

            command.Parameters.Add("@SlotDate", SqlDbType.Date).Value =
                booking.SlotDate.Date;

            command.Parameters.Add("@SlotTime", SqlDbType.NVarChar, 40).Value =
                booking.SlotTime;

            var depositParameter = command.Parameters.Add(
                "@DepositPaid", SqlDbType.Decimal);

            depositParameter.Precision = 18;
            depositParameter.Scale = 2;
            depositParameter.Value = booking.DepositPaid;

            command.Parameters.Add("@Status", SqlDbType.NVarChar, 40).Value =
                booking.Status;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 1000)
                .Value = (object?)booking.Description ?? DBNull.Value;
        }

        private static AmenityBooking MapBooking(SqlDataReader reader)
        {
            return new AmenityBooking
            {
                BookingId = Convert.ToInt64(reader["BookingId"]),

                AmenityId = Convert.ToInt64(reader["AmenityId"]),
                AmenityName = Convert.ToString(
                    reader["AmenityName"]) ?? string.Empty,

                FlatId = Convert.ToInt64(reader["FlatId"]),
                FlatNumber = Convert.ToString(
                    reader["FlatNumber"]) ?? string.Empty,
                WingName = Convert.ToString(
                    reader["WingName"]) ?? string.Empty,

                SlotDate = Convert.ToDateTime(reader["SlotDate"]),
                SlotTime = Convert.ToString(
                    reader["SlotTime"]) ?? string.Empty,

                DepositPaid = Convert.ToDecimal(reader["DepositPaid"]),
                Status = Convert.ToString(
                    reader["Status"]) ?? "Booked",

                Description = reader["Description"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["Description"]),

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
                    : Convert.ToInt64(reader["ModifyBy"]),

                TenantId = Convert.ToInt64(reader["TenantId"]),
                TenantName = Convert.ToString(
                    reader["TenantName"]) ?? string.Empty
            };
        }
    }
}
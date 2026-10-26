
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public AmenityRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Amenity>> GetAllAsync()
        {
            var amenities = new List<Amenity>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT AmenityId, Name, DepositRequired, MaxSlotHours,
                         Description, IsActive, CreatedOn, CreatedBy,
                         ModifyOn, ModifyBy, TenantId
                  FROM Amenities
                  WHERE IsActive = 1
                  ORDER BY AmenityId DESC",
                connection);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                amenities.Add(MapAmenity(reader));
            }

            return amenities;
        }

        public async Task<Amenity?> GetByIdAsync(long amenityId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT AmenityId, Name, DepositRequired, MaxSlotHours,
                         Description, IsActive, CreatedOn, CreatedBy,
                         ModifyOn, ModifyBy, TenantId
                  FROM Amenities
                  WHERE AmenityId = @AmenityId AND IsActive = 1",
                connection);

            command.Parameters.Add("@AmenityId", SqlDbType.BigInt).Value = amenityId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapAmenity(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Amenity amenity)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"INSERT INTO Amenities
                    (Name, DepositRequired, MaxSlotHours, Description,
                     IsActive, CreatedOn, CreatedBy, TenantId)
                  VALUES
                    (@Name, @DepositRequired, @MaxSlotHours, @Description,
                     @IsActive, @CreatedOn, @CreatedBy, @TenantId);
                  SELECT CAST(SCOPE_IDENTITY() AS BIGINT);",
                connection);

            AddParameters(command, amenity);

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = amenity.IsActive;
            command.Parameters.Add("@CreatedOn", SqlDbType.DateTime2).Value = amenity.CreatedOn;
            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)amenity.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            return Convert.ToInt64(await command.ExecuteScalarAsync());
        }

        public async Task<bool> UpdateAsync(Amenity amenity)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"UPDATE Amenities
                  SET Name = @Name,
                      DepositRequired = @DepositRequired,
                      MaxSlotHours = @MaxSlotHours,
                      Description = @Description,
                      ModifyOn = @ModifyOn,
                      ModifyBy = @ModifyBy
                  WHERE AmenityId = @AmenityId AND IsActive = 1",
                connection);

            AddParameters(command, amenity);

            command.Parameters.Add("@AmenityId", SqlDbType.BigInt).Value = amenity.AmenityId;
            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime2).Value =
                (object?)amenity.ModifyOn ?? DBNull.Value;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)amenity.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long amenityId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"UPDATE Amenities
                  SET IsActive = 0,
                      ModifyOn = @ModifyOn
                  WHERE AmenityId = @AmenityId AND IsActive = 1",
                connection);

            command.Parameters.Add("@AmenityId", SqlDbType.BigInt).Value = amenityId;
            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime2).Value = DateTime.UtcNow;

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static void AddParameters(SqlCommand command, Amenity amenity)
        {
            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = amenity.Name;

            var depositParameter = command.Parameters.Add("@DepositRequired", SqlDbType.Decimal);
            depositParameter.Precision = 18;
            depositParameter.Scale = 2;
            depositParameter.Value = amenity.DepositRequired;

            command.Parameters.Add("@MaxSlotHours", SqlDbType.Int).Value = amenity.MaxSlotHours;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)amenity.Description ?? DBNull.Value;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = amenity.TenantId;
        }

        private static Amenity MapAmenity(SqlDataReader reader)
        {
            return new Amenity
            {
                AmenityId = reader.GetInt64(reader.GetOrdinal("AmenityId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                DepositRequired = reader.GetDecimal(reader.GetOrdinal("DepositRequired")),
                MaxSlotHours = reader.GetInt32(reader.GetOrdinal("MaxSlotHours")),
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


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
                "dbo.GetAllAmenities", connection);

            command.CommandType = CommandType.StoredProcedure;

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
                "dbo.GetAmenityById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@AmenityId", SqlDbType.BigInt).Value =
                amenityId;

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
                "dbo.InsertAmenity", connection);

            command.CommandType = CommandType.StoredProcedure;

            AddAmenityParameters(command, amenity, isUpdate: false);

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Amenity amenity)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.UpdateAmenity", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@AmenityId", SqlDbType.BigInt).Value =
                amenity.AmenityId;

            AddAmenityParameters(command, amenity, isUpdate: true);

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(
            long amenityId, long? modifyBy = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.DeleteAmenity", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@AmenityId", SqlDbType.BigInt).Value =
                amenityId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        private static void AddAmenityParameters(
            SqlCommand command,
            Amenity amenity,
            bool isUpdate)
        {
            if (!isUpdate)
            {
                command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                    amenity.TenantId;
            }
            else
            {
                command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                    amenity.TenantId;
            }

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value =
                amenity.Name.Trim();

            var depositParameter = command.Parameters.Add(
                "@DepositRequired", SqlDbType.Decimal);

            depositParameter.Precision = 18;
            depositParameter.Scale = 2;
            depositParameter.Value = amenity.DepositRequired;

            command.Parameters.Add("@MaxSlotHours", SqlDbType.Int).Value =
                amenity.MaxSlotHours;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value =
                (object?)amenity.Description ?? DBNull.Value;

            if (isUpdate)
            {
                command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                    (object?)amenity.ModifyBy ?? DBNull.Value;
            }
            else
            {
                command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                    (object?)amenity.CreatedBy ?? DBNull.Value;
            }
        }

        private static Amenity MapAmenity(SqlDataReader reader)
        {
            return new Amenity
            {
                AmenityId = Convert.ToInt64(reader["AmenityId"]),
                Name = reader["Name"].ToString() ?? string.Empty,
                DepositRequired = Convert.ToDecimal(reader["DepositRequired"]),
                MaxSlotHours = Convert.ToInt32(reader["MaxSlotHours"]),
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
                    : Convert.ToInt64(reader["ModifyBy"]),
                TenantId = Convert.ToInt64(reader["TenantId"]),
                TenantName = reader["TenantName"].ToString() ?? string.Empty
            };
        }
    }
}
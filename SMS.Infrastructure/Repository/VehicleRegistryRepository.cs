
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class VehicleRegistryRepository : IVehicleRegistryRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public VehicleRegistryRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<VehicleRegistry>> GetAllAsync()
        {
            var vehicles = new List<VehicleRegistry>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.GetAllVehicleRegistry", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                vehicles.Add(MapVehicle(reader));
            }

            return vehicles;
        }

        public async Task<VehicleRegistry?> GetByIdAsync(long vehicleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.GetVehicleRegistryById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@VehicleId", SqlDbType.BigInt).Value = vehicleId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapVehicle(reader) : null;
        }

        public async Task<long> CreateAsync(VehicleRegistry vehicle)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.InsertVehicleRegistry", connection);

            command.CommandType = CommandType.StoredProcedure;
            AddVehicleParameters(command, vehicle, isUpdate: false);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(VehicleRegistry vehicle)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.UpdateVehicleRegistry", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@VehicleId", SqlDbType.BigInt).Value =
                vehicle.VehicleId;

            AddVehicleParameters(command, vehicle, isUpdate: true);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long vehicleId, long? modifyBy = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.DeleteVehicleRegistry", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@VehicleId", SqlDbType.BigInt).Value =
                vehicleId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToInt32(result) > 0;
        }

        public async Task<IEnumerable<VehicleFlatOption>> GetFlatsByTenantAsync(
            long tenantId)
        {
            var flats = new List<VehicleFlatOption>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.GetVehicleFlatsByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = tenantId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                flats.Add(new VehicleFlatOption
                {
                    FlatId = Convert.ToInt64(reader["FlatId"]),
                    FlatNumber = reader["FlatNumber"]?.ToString() ?? string.Empty,
                    WingId = Convert.ToInt64(reader["WingId"]),
                    WingName = reader["WingName"]?.ToString() ?? string.Empty
                });
            }

            return flats;
        }

        private static void AddVehicleParameters(
            SqlCommand command,
            VehicleRegistry vehicle,
            bool isUpdate)
        {
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                vehicle.TenantId;

            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value =
                vehicle.FlatId;

            command.Parameters.Add("@VehicleNumber", SqlDbType.NVarChar, 40).Value =
                vehicle.VehicleNumber.Trim().ToUpperInvariant();

            command.Parameters.Add("@VehicleType", SqlDbType.NVarChar, 40).Value =
                vehicle.VehicleType.Trim();

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value =
                (object?)vehicle.Description ?? DBNull.Value;

            string auditParameter = isUpdate ? "@ModifyBy" : "@CreatedBy";

            command.Parameters.Add(auditParameter, SqlDbType.BigInt).Value =
                (object?)(isUpdate ? vehicle.ModifyBy : vehicle.CreatedBy)
                ?? DBNull.Value;
        }

        private static VehicleRegistry MapVehicle(SqlDataReader reader)
        {
            return new VehicleRegistry
            {
                VehicleId = Convert.ToInt64(reader["VehicleId"]),
                TenantId = Convert.ToInt64(reader["TenantId"]),
                TenantName = reader["TenantName"]?.ToString() ?? string.Empty,
                FlatId = Convert.ToInt64(reader["FlatId"]),
                FlatNumber = reader["FlatNumber"]?.ToString() ?? string.Empty,
                WingName = reader["WingName"]?.ToString() ?? string.Empty,
                VehicleNumber = reader["VehicleNumber"]?.ToString() ?? string.Empty,
                VehicleType = reader["VehicleType"]?.ToString() ?? string.Empty,
                Description = reader["Description"] == DBNull.Value
                    ? null
                    : reader["Description"].ToString(),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value
                    ? null : Convert.ToInt64(reader["CreatedBy"]),
                ModifyOn = reader["ModifyOn"] == DBNull.Value
                    ? null : Convert.ToDateTime(reader["ModifyOn"]),
                ModifyBy = reader["ModifyBy"] == DBNull.Value
                    ? null : Convert.ToInt64(reader["ModifyBy"])
            };
        }
    }
}
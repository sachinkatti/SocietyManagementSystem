
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class ResidentRepository : IResidentRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ResidentRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Resident>> GetAllAsync()
        {
            var residents = new List<Resident>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllResidents", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                residents.Add(MapResident(reader));
            }

            return residents;
        }

        public async Task<Resident?> GetByIdAsync(long residentId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetResidentById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ResidentId", SqlDbType.BigInt).Value =
                residentId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapResident(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Resident resident)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("InsertResident", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value =
                resident.UserId;

            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value =
                resident.FlatId;

            command.Parameters.Add("@ResidentType", SqlDbType.NVarChar, 20).Value =
                resident.ResidentType;

            command.Parameters.Add("@MoveInDate", SqlDbType.Date).Value =
                resident.MoveInDate.Date;

            command.Parameters.Add("@EmergencyContact", SqlDbType.NVarChar, 15).Value =
                (object?)resident.EmergencyContact ?? DBNull.Value;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)resident.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Resident resident)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateResident", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ResidentId", SqlDbType.BigInt).Value =
                resident.ResidentId;

            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value =
                resident.UserId;

            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value =
                resident.FlatId;

            command.Parameters.Add("@ResidentType", SqlDbType.NVarChar, 20).Value =
                resident.ResidentType;

            command.Parameters.Add("@MoveInDate", SqlDbType.Date).Value =
                resident.MoveInDate.Date;

            command.Parameters.Add("@EmergencyContact", SqlDbType.NVarChar, 15).Value =
                (object?)resident.EmergencyContact ?? DBNull.Value;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)resident.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToBoolean(result);
        }

        public async Task<bool> DeleteAsync(long residentId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("DeleteResident", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ResidentId", SqlDbType.BigInt).Value =
                residentId;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToBoolean(result);
        }

        private static Resident MapResident(SqlDataReader reader)
        {
            return new Resident
            {
                ResidentId = reader.GetInt64(reader.GetOrdinal("ResidentId")),
                UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                FlatId = reader.GetInt64(reader.GetOrdinal("FlatId")),
                ResidentType = reader.GetString(reader.GetOrdinal("ResidentType")),
                MoveInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                EmergencyContact = reader.IsDBNull(reader.GetOrdinal("EmergencyContact"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("EmergencyContact")),
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
    }
}

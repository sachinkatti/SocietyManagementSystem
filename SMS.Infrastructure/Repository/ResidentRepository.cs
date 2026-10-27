
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
            using var command = new SqlCommand("dbo.GetAllResidents", connection);

            command.CommandType = CommandType.StoredProcedure;

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
            using var command = new SqlCommand(
                "dbo.GetResidentById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@ResidentId", SqlDbType.BigInt)
                .Value = residentId;

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
            using var command = new SqlCommand(
                "dbo.InsertResident", connection);

            command.CommandType = CommandType.StoredProcedure;
            AddResidentParameters(command, resident);

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt)
                .Value = resident.CreatedBy.HasValue
                    ? resident.CreatedBy.Value
                    : DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Resident resident)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.UpdateResident", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ResidentId", SqlDbType.BigInt).Value = resident.ResidentId;

            AddResidentParameters(command, resident);

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt)
                .Value = resident.ModifyBy.HasValue
                    ? resident.ModifyBy.Value
                    : DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long residentId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "dbo.DeleteResident", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@ResidentId", SqlDbType.BigInt).Value = residentId;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        private static void AddResidentParameters(
            SqlCommand command, Resident resident)
        {
            command.Parameters.Add("@UserId", SqlDbType.BigInt).Value = resident.UserId;
            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value = resident.FlatId;
            command.Parameters.Add("@ResidentType", SqlDbType.NVarChar, 20).Value = resident.ResidentType;
            command.Parameters.Add("@MoveInDate", SqlDbType.Date).Value = resident.MoveInDate.Date;
            command.Parameters.Add("@EmergencyContact", SqlDbType.NVarChar, 15)
                .Value = string.IsNullOrWhiteSpace(resident.EmergencyContact)
                    ? DBNull.Value
                    : resident.EmergencyContact;
        }

        private static Resident MapResident(SqlDataReader reader)
        {
            return new Resident
            {
                ResidentId = Convert.ToInt64(reader["ResidentId"]),

                UserId = Convert.ToInt64(reader["UserId"]),
                FullName = Convert.ToString(reader["FullName"]) ?? string.Empty,

                FlatId = Convert.ToInt64(reader["FlatId"]),
                FlatNumber = Convert.ToString(reader["FlatNumber"]) ?? string.Empty,
                WingName = Convert.ToString(reader["WingName"]) ?? string.Empty,

                ResidentType = Convert.ToString(reader["ResidentType"])
                    ?? string.Empty,

                MoveInDate = Convert.ToDateTime(reader["MoveInDate"]),

                EmergencyContact = reader["EmergencyContact"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["EmergencyContact"]),

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
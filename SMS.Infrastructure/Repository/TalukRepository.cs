using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class TalukRepository : ITalukRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TalukRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Taluk>> GetAllAsync()
        {
            var taluks = new List<Taluk>();

            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "GetAllTaluk",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                taluks.Add(MapTaluk(reader));
            }

            return taluks;
        }

        public async Task<Taluk?> GetByIdAsync(long talukId)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "GetByTalukId",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@TalukId",
                SqlDbType.BigInt).Value = talukId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapTaluk(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Taluk taluk)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "InsertTaluk",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@DistrictId",
                SqlDbType.BigInt).Value = taluk.DistrictId;

            command.Parameters.Add(
                "@TalukName",
                SqlDbType.NVarChar, 100).Value =
                taluk.TalukName;

            command.Parameters.Add(
                "@TalukCode",
                SqlDbType.NVarChar, 20).Value =
                taluk.TalukCode;

            command.Parameters.Add(
                "@Description",
                SqlDbType.NVarChar, 500).Value =
                (object?)taluk.Description ?? DBNull.Value;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit).Value =
                taluk.IsActive;

            command.Parameters.Add(
                "@CreatedBy",
                SqlDbType.BigInt).Value =
                (object?)taluk.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Taluk taluk)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "UpdateTaluk",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@TalukId",
                SqlDbType.BigInt).Value =
                taluk.TalukId;

            command.Parameters.Add(
                "@DistrictId",
                SqlDbType.BigInt).Value =
                taluk.DistrictId;

            command.Parameters.Add(
                "@TalukName",
                SqlDbType.NVarChar, 100).Value =
                taluk.TalukName;

            command.Parameters.Add(
                "@TalukCode",
                SqlDbType.NVarChar, 20).Value =
                taluk.TalukCode;

            command.Parameters.Add(
                "@Description",
                SqlDbType.NVarChar, 500).Value =
                (object?)taluk.Description ?? DBNull.Value;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit).Value =
                taluk.IsActive;

            command.Parameters.Add(
                "@ModifyBy",
                SqlDbType.BigInt).Value =
                (object?)taluk.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long talukId)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "DeleteTaluk",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@TalukId",
                SqlDbType.BigInt).Value =
                talukId;

            command.Parameters.Add(
                "@ModifyBy",
                SqlDbType.BigInt).Value = 1;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        private static Taluk MapTaluk(SqlDataReader reader)
        {
            return new Taluk
            {
                TalukId = Convert.ToInt64(
                    reader["TalukId"]),

                DistrictId = Convert.ToInt64(
                    reader["DistrictId"]),

                TalukName = reader["TalukName"]?.ToString()
                    ?? string.Empty,

                TalukCode = reader["TalukCode"]?.ToString()
                    ?? string.Empty,

                Description = reader["Description"] == DBNull.Value
                    ? null
                    : reader["Description"].ToString(),

                IsActive = Convert.ToBoolean(
                    reader["IsActive"]),

                CreatedOn = reader["CreatedOn"] == DBNull.Value
                    ? default
                    : Convert.ToDateTime(reader["CreatedOn"]),

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
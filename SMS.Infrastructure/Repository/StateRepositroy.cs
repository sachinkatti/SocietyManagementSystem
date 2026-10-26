using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class StateRepository : IStateRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public StateRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            var states = new List<State>();

            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "GetAllState",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                states.Add(MapState(reader));
            }

            return states;
        }

        public async Task<State?> GetByIdAsync(long stateId)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "GetByStateId",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@StateId",
                SqlDbType.BigInt).Value = stateId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapState(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(State state)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "InsertState",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@CountryId",
                SqlDbType.BigInt).Value = state.CountryId;

            command.Parameters.Add(
                "@StateName",
                SqlDbType.VarChar, 100).Value =
                state.StateName;

            command.Parameters.Add(
                "@StateCode",
                SqlDbType.VarChar, 20).Value =
                state.StateCode;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit).Value =
                state.IsActive;

            command.Parameters.Add(
                "@CreatedBy",
                SqlDbType.BigInt).Value =
                (object?)state.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(State state)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "UpdateState",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@StateId",
                SqlDbType.BigInt).Value =
                state.StateId;

            command.Parameters.Add(
                "@CountryId",
                SqlDbType.BigInt).Value =
                state.CountryId;

            command.Parameters.Add(
                "@StateName",
                SqlDbType.VarChar, 100).Value =
                state.StateName;

            command.Parameters.Add(
                "@StateCode",
                SqlDbType.VarChar, 20).Value =
                state.StateCode;

            command.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit).Value =
                state.IsActive;

            command.Parameters.Add(
                "@ModifyBy",
                SqlDbType.BigInt).Value =
                (object?)state.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long stateId)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "DeleteState",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                "@StateId",
                SqlDbType.BigInt).Value = stateId;

            command.Parameters.Add(
                "@ModifyBy",
                SqlDbType.BigInt).Value = 1;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        private static State MapState(SqlDataReader reader)
        {
            return new State
            {
                StateId = Convert.ToInt64(reader["StateId"]),
                CountryId = Convert.ToInt64(reader["CountryId"]),
                StateName = reader["StateName"]?.ToString() ?? string.Empty,
                StateCode = reader["StateCode"]?.ToString() ?? string.Empty,
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
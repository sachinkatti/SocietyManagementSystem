
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class ExpenseLedgerRepository : IExpenseLedgerRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ExpenseLedgerRepository(
            DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // 1. Get all active expense ledger entries
        public async Task<List<ExpenseLedger>> GetAllAsync()
        {
            var entries = new List<ExpenseLedger>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetAllExpenseLedger", connection);

            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                entries.Add(MapExpenseLedger(reader));
            }

            return entries;
        }

        // 2. Get one entry by ID
        public async Task<ExpenseLedger?> GetByIdAsync(long ledgerId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetExpenseLedgerById", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@LedgerId", SqlDbType.BigInt)
                .Value = ledgerId;

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapExpenseLedger(reader);
            }

            return null;
        }

        // 3. Get entries for a specific tenant
        public async Task<List<ExpenseLedger>> GetByTenantAsync(
            long tenantId)
        {
            var entries = new List<ExpenseLedger>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetExpenseLedgerByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt)
                .Value = tenantId;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                entries.Add(MapExpenseLedger(reader));
            }

            return entries;
        }

        // 4. Get administrators for a tenant
        public async Task<List<ExpenseLedger>> GetAdminsByTenantAsync(
            long tenantId)
        {
            var admins = new List<ExpenseLedger>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetExpenseLedgerAdminsByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt)
                .Value = tenantId;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                admins.Add(new ExpenseLedger
                {
                    RecordedByAdminId =
                        Convert.ToInt64(reader["AdminId"]),

                    RecordedByAdminName =
                        Convert.ToString(reader["FullName"]),

                    Designation =
                        Convert.ToString(reader["Designation"])
                });
            }

            return admins;
        }

        // 5. Create a new entry
        public async Task<long> CreateAsync(ExpenseLedger expenseLedger)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.InsertExpenseLedger", connection);

            command.CommandType = CommandType.StoredProcedure;

            AddEntryParameters(command, expenseLedger);

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt)
                .Value = (object?)expenseLedger.CreatedBy ?? DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt)
                .Value = expenseLedger.TenantId;

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        // 6. Update an existing entry

        public async Task<bool> UpdateAsync(ExpenseLedger expenseLedger)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.UpdateExpenseLedger", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@LedgerId", SqlDbType.BigInt)
                .Value = expenseLedger.LedgerId;

            command.Parameters.Add("@EntryType", SqlDbType.NVarChar, 10)
                .Value = expenseLedger.EntryType;

            command.Parameters.Add("@Category", SqlDbType.NVarChar, 50)
                .Value = expenseLedger.Category;

            var amountParameter = command.Parameters.Add(
                "@Amount", SqlDbType.Decimal);

            amountParameter.Precision = 18;
            amountParameter.Scale = 2;
            amountParameter.Value = expenseLedger.Amount;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 250)
                .Value = (object?)expenseLedger.Description ?? DBNull.Value;

            command.Parameters.Add("@EntryDate", SqlDbType.Date)
                .Value = expenseLedger.EntryDate.Date;

            command.Parameters.Add("@RecordedByAdminId", SqlDbType.BigInt)
                .Value = expenseLedger.RecordedByAdminId;

            command.Parameters.Add("@IsActive", SqlDbType.Bit)
                .Value = expenseLedger.IsActive;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt)
                .Value = (object?)expenseLedger.ModifyBy ?? DBNull.Value;

            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime)
                .Value = DateTime.Now;

            await command.ExecuteNonQueryAsync();

            return true;
        }


        // 7. Soft-delete an entry
        public async Task<bool> DeleteAsync(
            long ledgerId,
            long? modifyBy,
            long tenantId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.DeleteExpenseLedger", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@LedgerId", SqlDbType.BigInt)
                .Value = ledgerId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt)
                .Value = (object?)modifyBy ?? DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt)
                .Value = tenantId;

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        // Shared parameters for insert and update
        private static void AddEntryParameters(
            SqlCommand command,
            ExpenseLedger expenseLedger)
        {
            command.Parameters.Add("@EntryType", SqlDbType.NVarChar, 20)
                .Value = expenseLedger.EntryType;

            command.Parameters.Add("@Category", SqlDbType.NVarChar, 100)
                .Value = expenseLedger.Category;

            var amountParameter = command.Parameters.Add(
                "@Amount", SqlDbType.Decimal);

            amountParameter.Precision = 18;
            amountParameter.Scale = 2;
            amountParameter.Value = expenseLedger.Amount;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500)
                .Value = (object?)expenseLedger.Description ?? DBNull.Value;

            command.Parameters.Add("@EntryDate", SqlDbType.Date)
                .Value = expenseLedger.EntryDate.Date;

            command.Parameters.Add("@RecordedByAdminId", SqlDbType.BigInt)
                .Value = expenseLedger.RecordedByAdminId;
        }

        // Map database columns to the entity
        private static ExpenseLedger MapExpenseLedger(SqlDataReader reader)
        {
            return new ExpenseLedger
            {
                LedgerId = Convert.ToInt64(reader["LedgerId"]),
                EntryType = Convert.ToString(reader["EntryType"]) ?? string.Empty,
                Category = Convert.ToString(reader["Category"]) ?? string.Empty,
                Amount = Convert.ToDecimal(reader["Amount"]),
                Description = reader["Description"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["Description"]),

                EntryDate = Convert.ToDateTime(reader["EntryDate"]),
                RecordedByAdminId =
                    Convert.ToInt64(reader["RecordedByAdminId"]),

                RecordedByAdminName =
                    reader["RecordedByAdminName"] == DBNull.Value
                        ? null
                        : Convert.ToString(reader["RecordedByAdminName"]),

                Designation = reader["Designation"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["Designation"]),

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

                TenantName = reader["TenantName"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["TenantName"])
            };
        }
    }
}
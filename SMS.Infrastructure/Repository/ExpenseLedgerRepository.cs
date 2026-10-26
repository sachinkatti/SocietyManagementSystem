using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class ExpenseLedgerRepository : IExpenseLedgerRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ExpenseLedgerRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ExpenseLedger>> GetAllAsync()
        {
            var ledgers = new List<ExpenseLedger>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT LedgerId, EntryType, Category, Amount, Description,
                         EntryDate, RecordedByAdminId, IsActive, CreatedOn,
                         CreatedBy, ModifyOn, ModifyBy, TenantId
                  FROM ExpenseLedger
                  WHERE IsActive = 1
                  ORDER BY LedgerId DESC",
                connection);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ledgers.Add(MapLedger(reader));
            }

            return ledgers;
        }

        public async Task<ExpenseLedger?> GetByIdAsync(long ledgerId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT LedgerId, EntryType, Category, Amount, Description,
                         EntryDate, RecordedByAdminId, IsActive, CreatedOn,
                         CreatedBy, ModifyOn, ModifyBy, TenantId
                  FROM ExpenseLedger
                  WHERE LedgerId = @LedgerId AND IsActive = 1",
                connection);

            command.Parameters.Add("@LedgerId", SqlDbType.BigInt).Value = ledgerId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapLedger(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(ExpenseLedger ledger)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"INSERT INTO ExpenseLedger
                    (EntryType, Category, Amount, Description, EntryDate,
                     RecordedByAdminId, IsActive, CreatedOn, CreatedBy,
                     ModifyOn, ModifyBy, TenantId)
                  VALUES
                    (@EntryType, @Category, @Amount, @Description, @EntryDate,
                     @RecordedByAdminId, @IsActive, @CreatedOn, @CreatedBy,
                     @ModifyOn, @ModifyBy, @TenantId);
                  SELECT CAST(SCOPE_IDENTITY() AS BIGINT);",
                connection);

            AddParameters(command, ledger, false);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(ExpenseLedger ledger)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"UPDATE ExpenseLedger
                  SET EntryType = @EntryType,
                      Category = @Category,
                      Amount = @Amount,
                      Description = @Description,
                      EntryDate = @EntryDate,
                      RecordedByAdminId = @RecordedByAdminId,
                      ModifyOn = @ModifyOn,
                      ModifyBy = @ModifyBy,
                      TenantId = @TenantId
                  WHERE LedgerId = @LedgerId AND IsActive = 1",
                connection);

            AddParameters(command, ledger, true);

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long ledgerId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"UPDATE ExpenseLedger
                  SET IsActive = 0, ModifyOn = @ModifyOn
                  WHERE LedgerId = @LedgerId AND IsActive = 1",
                connection);

            command.Parameters.Add("@LedgerId", SqlDbType.BigInt).Value = ledgerId;
            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime2).Value = DateTime.UtcNow;

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static void AddParameters(SqlCommand command, ExpenseLedger ledger, bool includeId)
        {
            if (includeId)
            {
                command.Parameters.Add("@LedgerId", SqlDbType.BigInt).Value = ledger.LedgerId;
            }

            command.Parameters.Add("@EntryType", SqlDbType.NVarChar, 10).Value = ledger.EntryType;
            command.Parameters.Add("@Category", SqlDbType.NVarChar, 50).Value = ledger.Category;

            var amountParameter = command.Parameters.Add("@Amount", SqlDbType.Decimal);
            amountParameter.Precision = 18;
            amountParameter.Scale = 2;
            amountParameter.Value = ledger.Amount;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 250).Value =
                (object?)ledger.Description ?? DBNull.Value;

            command.Parameters.Add("@EntryDate", SqlDbType.Date).Value = ledger.EntryDate.Date;
            command.Parameters.Add("@RecordedByAdminId", SqlDbType.BigInt).Value = ledger.RecordedByAdminId;

            if (!includeId)
            {
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = true;
                command.Parameters.Add("@CreatedOn", SqlDbType.DateTime2).Value = DateTime.UtcNow;
                command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                    (object?)ledger.CreatedBy ?? DBNull.Value;
                command.Parameters.Add("@ModifyOn", SqlDbType.DateTime2).Value = DBNull.Value;
                command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value = DBNull.Value;
            }
            else
            {
                command.Parameters.Add("@ModifyOn", SqlDbType.DateTime2).Value = DateTime.UtcNow;
                command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                    (object?)ledger.ModifyBy ?? DBNull.Value;
            }

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = ledger.TenantId;
        }

        private static ExpenseLedger MapLedger(SqlDataReader reader)
        {
            return new ExpenseLedger
            {
                LedgerId = reader.GetInt64(reader.GetOrdinal("LedgerId")),
                EntryType = reader.GetString(reader.GetOrdinal("EntryType")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
                EntryDate = reader.GetDateTime(reader.GetOrdinal("EntryDate")),
                RecordedByAdminId = reader.GetInt64(reader.GetOrdinal("RecordedByAdminId")),
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
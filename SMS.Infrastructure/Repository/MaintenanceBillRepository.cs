using Microsoft.Data.SqlClient;
using SMS.Domain;

namespace SMS.Infrastructure
{
    public class MaintenanceBillRepository : IMaintenanceBillRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public MaintenanceBillRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<MaintenanceBill>> GetAllAsync()
        {
            var bills = new List<MaintenanceBill>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT BillId, FlatId, BillingMonth, BillingYear,
                       BaseAmount, PenaltyAmount, TotalAmount, DueDate,
                       PaymentStatus, PaymentDate, TransactionRef,
                       Description, IsActive, CreatedOn, CreatedBy,
                       ModifyOn, ModifyBy, TenantId
                FROM MaintenanceBills
                WHERE IsActive = 1
                ORDER BY BillingYear DESC, BillingMonth DESC, BillId DESC";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                bills.Add(MapBill(reader));
            }

            return bills;
        }

        public async Task<MaintenanceBill?> GetByIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT BillId, FlatId, BillingMonth, BillingYear,
                       BaseAmount, PenaltyAmount, TotalAmount, DueDate,
                       PaymentStatus, PaymentDate, TransactionRef,
                       Description, IsActive, CreatedOn, CreatedBy,
                       ModifyOn, ModifyBy, TenantId
                FROM MaintenanceBills
                WHERE BillId = @BillId AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BillId", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapBill(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(MaintenanceBill bill)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO MaintenanceBills
                (
                    FlatId, BillingMonth, BillingYear, BaseAmount,
                    PenaltyAmount, TotalAmount, DueDate, PaymentStatus,
                    PaymentDate, TransactionRef, Description, IsActive,
                    CreatedOn, CreatedBy, ModifyOn, ModifyBy, TenantId
                )
                VALUES
                (
                    @FlatId, @BillingMonth, @BillingYear, @BaseAmount,
                    @PenaltyAmount, @TotalAmount, @DueDate, @PaymentStatus,
                    @PaymentDate, @TransactionRef, @Description, @IsActive,
                    @CreatedOn, @CreatedBy, @ModifyOn, @ModifyBy, @TenantId
                );

                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using var command = new SqlCommand(query, connection);
            AddParameters(command, bill);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(MaintenanceBill bill)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                UPDATE MaintenanceBills
                SET FlatId = @FlatId,
                    BillingMonth = @BillingMonth,
                    BillingYear = @BillingYear,
                    BaseAmount = @BaseAmount,
                    PenaltyAmount = @PenaltyAmount,
                    TotalAmount = @TotalAmount,
                    DueDate = @DueDate,
                    PaymentStatus = @PaymentStatus,
                    PaymentDate = @PaymentDate,
                    TransactionRef = @TransactionRef,
                    Description = @Description,
                    ModifyOn = @ModifyOn,
                    ModifyBy = @ModifyBy,
                    TenantId = @TenantId
                WHERE BillId = @BillId AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            AddParameters(command, bill);
            command.Parameters.AddWithValue("@BillId", bill.BillId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                UPDATE MaintenanceBills
                SET IsActive = 0,
                    ModifyOn = @ModifyOn
                WHERE BillId = @BillId AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BillId", id);
            command.Parameters.AddWithValue("@ModifyOn", DateTime.UtcNow);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static void AddParameters(
            SqlCommand command,
            MaintenanceBill bill)
        {
            command.Parameters.AddWithValue("@FlatId", bill.FlatId);
            command.Parameters.AddWithValue("@BillingMonth", bill.BillingMonth);
            command.Parameters.AddWithValue("@BillingYear", bill.BillingYear);
            command.Parameters.AddWithValue("@BaseAmount", bill.BaseAmount);
            command.Parameters.AddWithValue("@PenaltyAmount", bill.PenaltyAmount);
            command.Parameters.AddWithValue("@TotalAmount", bill.TotalAmount);
            command.Parameters.AddWithValue("@DueDate", bill.DueDate.Date);
            command.Parameters.AddWithValue("@PaymentStatus", bill.PaymentStatus);
            command.Parameters.AddWithValue(
                "@PaymentDate",
                (object?)bill.PaymentDate?.Date ?? DBNull.Value);
            command.Parameters.AddWithValue(
                "@TransactionRef",
                (object?)bill.TransactionRef ?? DBNull.Value);
            command.Parameters.AddWithValue(
                "@Description",
                (object?)bill.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", bill.IsActive);
            command.Parameters.AddWithValue("@CreatedOn", bill.CreatedOn);
            command.Parameters.AddWithValue(
                "@CreatedBy",
                (object?)bill.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue(
                "@ModifyOn",
                (object?)bill.ModifyOn ?? DBNull.Value);
            command.Parameters.AddWithValue(
                "@ModifyBy",
                (object?)bill.ModifyBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@TenantId", bill.TenantId);
        }

        private static MaintenanceBill MapBill(SqlDataReader reader)
        {
            return new MaintenanceBill
            {
                BillId = reader.GetInt64(reader.GetOrdinal("BillId")),
                FlatId = reader.GetInt64(reader.GetOrdinal("FlatId")),
                BillingMonth = reader.GetInt32(reader.GetOrdinal("BillingMonth")),
                BillingYear = reader.GetInt32(reader.GetOrdinal("BillingYear")),
                BaseAmount = reader.GetDecimal(reader.GetOrdinal("BaseAmount")),
                PenaltyAmount = reader.GetDecimal(reader.GetOrdinal("PenaltyAmount")),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                PaymentStatus = reader.GetString(reader.GetOrdinal("PaymentStatus")),
                PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
                TransactionRef = reader.IsDBNull(reader.GetOrdinal("TransactionRef"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("TransactionRef")),
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
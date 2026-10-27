
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class MaintenanceBillRepository : IMaintenanceBillRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public MaintenanceBillRepository(
            DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<MaintenanceBill>> GetAllAsync()
        {
            var bills = new List<MaintenanceBill>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetAllMaintenanceBills", connection);

            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                bills.Add(MapMaintenanceBill(reader));
            }

            return bills;
        }

        public async Task<MaintenanceBill?> GetByIdAsync(long billId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetMaintenanceBillById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@BillId", SqlDbType.BigInt).Value = billId;

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapMaintenanceBill(reader);
            }

            return null;
        }

        public async Task<List<MaintenanceBill>> GetByTenantAsync(long tenantId)
        {
            var bills = new List<MaintenanceBill>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetMaintenanceBillsByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = tenantId;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                bills.Add(MapMaintenanceBill(reader));
            }

            return bills;
        }

        public async Task<long> CreateAsync(MaintenanceBill bill)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.InsertMaintenanceBill", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value = bill.FlatId;
            command.Parameters.Add("@BillingMonth", SqlDbType.Int).Value = bill.BillingMonth;
            command.Parameters.Add("@BillingYear", SqlDbType.Int).Value = bill.BillingYear;

            command.Parameters.Add("@BaseAmount", SqlDbType.Decimal).Value = bill.BaseAmount;
            command.Parameters["@BaseAmount"].Precision = 18;
            command.Parameters["@BaseAmount"].Scale = 2;

            command.Parameters.Add("@PenaltyAmount", SqlDbType.Decimal).Value = bill.PenaltyAmount;
            command.Parameters["@PenaltyAmount"].Precision = 18;
            command.Parameters["@PenaltyAmount"].Scale = 2;

            command.Parameters.Add("@DueDate", SqlDbType.Date).Value = bill.DueDate.Date;

            command.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar, 20).Value =
                bill.PaymentStatus;

            command.Parameters.Add("@PaymentDate", SqlDbType.Date).Value =
                (object?)bill.PaymentDate?.Date ?? DBNull.Value;

            command.Parameters.Add("@TransactionRef", SqlDbType.NVarChar, 100).Value =
                (object?)bill.TransactionRef ?? DBNull.Value;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)bill.Description ?? DBNull.Value;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)bill.CreatedBy ?? DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = bill.TenantId;

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(MaintenanceBill bill)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.UpdateMaintenanceBill", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@BillId", SqlDbType.BigInt).Value = bill.BillId;
            command.Parameters.Add("@FlatId", SqlDbType.BigInt).Value = bill.FlatId;
            command.Parameters.Add("@BillingMonth", SqlDbType.Int).Value = bill.BillingMonth;
            command.Parameters.Add("@BillingYear", SqlDbType.Int).Value = bill.BillingYear;

            command.Parameters.Add("@BaseAmount", SqlDbType.Decimal).Value = bill.BaseAmount;
            command.Parameters["@BaseAmount"].Precision = 18;
            command.Parameters["@BaseAmount"].Scale = 2;

            command.Parameters.Add("@PenaltyAmount", SqlDbType.Decimal).Value = bill.PenaltyAmount;
            command.Parameters["@PenaltyAmount"].Precision = 18;
            command.Parameters["@PenaltyAmount"].Scale = 2;

            command.Parameters.Add("@DueDate", SqlDbType.Date).Value = bill.DueDate.Date;

            command.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar, 20).Value =
                bill.PaymentStatus;

            command.Parameters.Add("@PaymentDate", SqlDbType.Date).Value =
                (object?)bill.PaymentDate?.Date ?? DBNull.Value;

            command.Parameters.Add("@TransactionRef", SqlDbType.NVarChar, 100).Value =
                (object?)bill.TransactionRef ?? DBNull.Value;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)bill.Description ?? DBNull.Value;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)bill.ModifyBy ?? DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = bill.TenantId;

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(
            long billId,
            long? modifyBy,
            long tenantId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.DeleteMaintenanceBill", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@BillId", SqlDbType.BigInt).Value = billId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = tenantId;

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }


        public async Task<List<MaintenanceBill>> GetFlatsByTenantAsync(long tenantId)
        {
            var flats = new List<MaintenanceBill>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetMaintenanceBillFlatsByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = tenantId;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                flats.Add(new MaintenanceBill
                {
                    FlatId = Convert.ToInt64(reader["FlatId"]),
                    FlatNumber = Convert.ToString(reader["FlatNumber"])
                });
            }

            return flats;
        }

        private static MaintenanceBill MapMaintenanceBill(SqlDataReader reader)
        {
            return new MaintenanceBill
            {
                BillId = Convert.ToInt64(reader["BillId"]),
                FlatId = Convert.ToInt64(reader["FlatId"]),
                FlatNumber = reader["FlatNumber"] == DBNull.Value
                    ? null : Convert.ToString(reader["FlatNumber"]),
                BillingMonth = Convert.ToInt32(reader["BillingMonth"]),
                BillingYear = Convert.ToInt32(reader["BillingYear"]),
                BaseAmount = Convert.ToDecimal(reader["BaseAmount"]),
                PenaltyAmount = Convert.ToDecimal(reader["PenaltyAmount"]),
                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                DueDate = Convert.ToDateTime(reader["DueDate"]),
                PaymentStatus = Convert.ToString(reader["PaymentStatus"]) ?? "Pending",
                PaymentDate = reader["PaymentDate"] == DBNull.Value
                    ? null : Convert.ToDateTime(reader["PaymentDate"]),
                TransactionRef = reader["TransactionRef"] == DBNull.Value
                    ? null : Convert.ToString(reader["TransactionRef"]),
                Description = reader["Description"] == DBNull.Value
                    ? null : Convert.ToString(reader["Description"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value
                    ? null : Convert.ToInt64(reader["CreatedBy"]),
                ModifyOn = reader["ModifyOn"] == DBNull.Value
                    ? null : Convert.ToDateTime(reader["ModifyOn"]),
                ModifyBy = reader["ModifyBy"] == DBNull.Value
                    ? null : Convert.ToInt64(reader["ModifyBy"]),
                TenantId = Convert.ToInt64(reader["TenantId"]),
                TenantName = reader["TenantName"] == DBNull.Value
                    ? null : Convert.ToString(reader["TenantName"])
            };
        }
    }
}
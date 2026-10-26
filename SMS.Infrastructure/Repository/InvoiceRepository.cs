using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public InvoiceRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private static Invoice MapInvoice(SqlDataReader reader)
        {
            return new Invoice
            {
                InvoiceId = reader.GetInt64(reader.GetOrdinal("InvoiceId")),
                SubscriptionId = reader.GetInt64(reader.GetOrdinal("SubscriptionId")),
                TenantId = reader.GetInt64(reader.GetOrdinal("TenantId")),
                InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                InvoiceDate = reader.GetDateTime(reader.GetOrdinal("InvoiceDate")),
                DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedOn = reader.IsDBNull(reader.GetOrdinal("CreatedOn")) ? default
                    : reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
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

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            var invoices = new List<Invoice>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllInvoice", connection);

            command.CommandType = CommandType.StoredProcedure;
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                invoices.Add(MapInvoice(reader));
            }

            return invoices;
        }

        public async Task<Invoice?> GetByIdAsync(long invoiceId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetByInvoiceId", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@InvoiceId", SqlDbType.BigInt).Value = invoiceId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapInvoice(reader) : null;
        }

        public async Task<long> CreateAsync(Invoice entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("InsertInvoice", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@SubscriptionId", SqlDbType.BigInt).Value = entity.SubscriptionId;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = entity.TenantId;
            command.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar, 50).Value = entity.InvoiceNumber;

            var amountParam = command.Parameters.Add("@Amount", SqlDbType.Decimal);
            amountParam.Precision = 18;
            amountParam.Scale = 2;
            amountParam.Value = entity.Amount;

            var taxParam = command.Parameters.Add("@TaxAmount", SqlDbType.Decimal);
            taxParam.Precision = 18;
            taxParam.Scale = 2;
            taxParam.Value = entity.TaxAmount;

            var totalParam = command.Parameters.Add("@TotalAmount", SqlDbType.Decimal);
            totalParam.Precision = 18;
            totalParam.Scale = 2;
            totalParam.Value = entity.TotalAmount;

            command.Parameters.Add("@InvoiceDate", SqlDbType.DateTime).Value = entity.InvoiceDate;
            command.Parameters.Add("@DueDate", SqlDbType.DateTime).Value = entity.DueDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = entity.Status;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)entity.Description ?? DBNull.Value;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)entity.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Invoice entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateInvoice", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@InvoiceId", SqlDbType.BigInt).Value = entity.InvoiceId;
            command.Parameters.Add("@SubscriptionId", SqlDbType.BigInt).Value = entity.SubscriptionId;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = entity.TenantId;
            command.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar, 50).Value = entity.InvoiceNumber;

            var amountParam = command.Parameters.Add("@Amount", SqlDbType.Decimal);
            amountParam.Precision = 18;
            amountParam.Scale = 2;
            amountParam.Value = entity.Amount;

            var taxParam = command.Parameters.Add("@TaxAmount", SqlDbType.Decimal);
            taxParam.Precision = 18;
            taxParam.Scale = 2;
            taxParam.Value = entity.TaxAmount;

            var totalParam = command.Parameters.Add("@TotalAmount", SqlDbType.Decimal);
            totalParam.Precision = 18;
            totalParam.Scale = 2;
            totalParam.Value = entity.TotalAmount;

            command.Parameters.Add("@InvoiceDate", SqlDbType.DateTime).Value = entity.InvoiceDate;
            command.Parameters.Add("@DueDate", SqlDbType.DateTime).Value = entity.DueDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = entity.Status;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)entity.Description ?? DBNull.Value;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)entity.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long invoiceId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("DeleteInvoice", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@InvoiceId", SqlDbType.BigInt).Value = invoiceId;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
    }
}
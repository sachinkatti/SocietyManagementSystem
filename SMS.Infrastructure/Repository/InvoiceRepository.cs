
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

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            var invoices = new List<Invoice>();

            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new SqlCommand(
                "dbo.GetAllInvoices", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                invoices.Add(MapInvoice(reader));
            }

            return invoices;
        }

        public async Task<Invoice?> GetByIdAsync(long invoiceId)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new SqlCommand(
                "dbo.GetInvoiceById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@InvoiceId", SqlDbType.BigInt).Value = invoiceId;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapInvoice(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Invoice invoice)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new SqlCommand(
                "dbo.InsertInvoice", connection);

            command.CommandType = CommandType.StoredProcedure;

            AddInvoiceParameters(command, invoice);

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Invoice invoice)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new SqlCommand(
                "dbo.UpdateInvoice", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@InvoiceId", SqlDbType.BigInt).Value =
                invoice.InvoiceId;

            AddInvoiceParameters(command, invoice);

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToBoolean(result);
        }

        public async Task<bool> DeleteAsync(long invoiceId)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new SqlCommand(
                "dbo.DeleteInvoice", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@InvoiceId", SqlDbType.BigInt).Value = invoiceId;

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToBoolean(result);
        }

        private static void AddInvoiceParameters(
            SqlCommand command, Invoice invoice)
        {
            command.Parameters.Add("@SubscriptionId", SqlDbType.BigInt).Value =
                invoice.SubscriptionId;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                invoice.TenantId;

            command.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar, 50).Value =
                invoice.InvoiceNumber;

            command.Parameters.Add("@Amount", SqlDbType.Decimal).Value =
                invoice.Amount;

            command.Parameters["@Amount"].Precision = 18;
            command.Parameters["@Amount"].Scale = 2;

            command.Parameters.Add("@TaxAmount", SqlDbType.Decimal).Value =
                invoice.TaxAmount;

            command.Parameters["@TaxAmount"].Precision = 18;
            command.Parameters["@TaxAmount"].Scale = 2;

            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value =
                invoice.TotalAmount;

            command.Parameters["@TotalAmount"].Precision = 18;
            command.Parameters["@TotalAmount"].Scale = 2;

            command.Parameters.Add("@InvoiceDate", SqlDbType.DateTime).Value =
                invoice.InvoiceDate;

            command.Parameters.Add("@DueDate", SqlDbType.DateTime).Value =
                invoice.DueDate;

            command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value =
                invoice.Status;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)invoice.Description ?? DBNull.Value;
        }

        private static Invoice MapInvoice(SqlDataReader reader)
        {
            return new Invoice
            {
                InvoiceId = Convert.ToInt64(reader["InvoiceId"]),
                SubscriptionId = Convert.ToInt64(reader["SubscriptionId"]),
                TenantId = Convert.ToInt64(reader["TenantId"]),
                InvoiceNumber = Convert.ToString(reader["InvoiceNumber"]) ?? string.Empty,
                Amount = Convert.ToDecimal(reader["Amount"]),
                TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
                DueDate = Convert.ToDateTime(reader["DueDate"]),
                Status = Convert.ToString(reader["Status"]) ?? "Pending",
                Description = reader["Description"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["Description"])
            };
        }
    }
}

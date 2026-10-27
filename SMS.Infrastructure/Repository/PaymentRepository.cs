
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public PaymentRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            var payments = new List<Payment>();

            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new SqlCommand(
                "dbo.GetAllPayments", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                payments.Add(MapPayment(reader));
            }

            return payments;
        }

        public async Task<Payment?> GetByIdAsync(long paymentId)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new SqlCommand(
                "dbo.GetPaymentById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@PaymentId", SqlDbType.BigInt).Value =
                paymentId;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapPayment(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Payment payment)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new SqlCommand(
                "dbo.InsertPayment", connection);

            command.CommandType = CommandType.StoredProcedure;

            AddPaymentParameters(command, payment);

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Payment payment)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new SqlCommand(
                "dbo.UpdatePayment", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PaymentId", SqlDbType.BigInt).Value =
                payment.PaymentId;

            AddPaymentParameters(command, payment);

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToBoolean(result);
        }

        public async Task<bool> DeleteAsync(long paymentId)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new SqlCommand(
                "dbo.DeletePayment", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@PaymentId", SqlDbType.BigInt).Value =
                paymentId;

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToBoolean(result);
        }

        private static void AddPaymentParameters(
            SqlCommand command, Payment payment)
        {
            command.Parameters.Add("@InvoiceId", SqlDbType.BigInt).Value =
                payment.InvoiceId;

            command.Parameters.Add("@PaymentReference", SqlDbType.NVarChar, 100).Value =
                payment.PaymentReference;

            command.Parameters.Add("@PaymentDate", SqlDbType.DateTime).Value =
                payment.PaymentDate;

            var amountParameter =
                command.Parameters.Add("@Amount", SqlDbType.Decimal);

            amountParameter.Precision = 18;
            amountParameter.Scale = 2;
            amountParameter.Value = payment.Amount;

            command.Parameters.Add("@PaymentMethod", SqlDbType.NVarChar, 30).Value =
                payment.PaymentMethod;

            command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value =
                payment.Status;

            command.Parameters.Add("@TransactionId", SqlDbType.NVarChar, 100).Value =
                (object?)payment.TransactionId ?? DBNull.Value;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)payment.Description ?? DBNull.Value;
        }

        private static Payment MapPayment(SqlDataReader reader)
        {
            return new Payment
            {
                PaymentId = Convert.ToInt64(reader["PaymentId"]),
                InvoiceId = Convert.ToInt64(reader["InvoiceId"]),
                PaymentReference =
                    Convert.ToString(reader["PaymentReference"]) ?? string.Empty,
                PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                Amount = Convert.ToDecimal(reader["Amount"]),
                PaymentMethod =
                    Convert.ToString(reader["PaymentMethod"]) ?? string.Empty,
                Status = Convert.ToString(reader["Status"]) ?? "Pending",
                TransactionId = reader["TransactionId"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["TransactionId"]),
                Description = reader["Description"] == DBNull.Value
                    ? null
                    : Convert.ToString(reader["Description"]),
                InvoiceNumber = Convert.ToString(reader["InvoiceNumber"])
            };
        }
    }
}
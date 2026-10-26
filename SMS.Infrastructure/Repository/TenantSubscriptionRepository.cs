using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class TenantSubscriptionRepository : ITenantSubscriptionRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TenantSubscriptionRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private static TenantSubscription MapSubscription(SqlDataReader reader)
        {
            return new TenantSubscription
            {
                SubscriptionId = reader.GetInt64(reader.GetOrdinal("SubscriptionId")),
                TenantId = reader.GetInt64(reader.GetOrdinal("TenantId")),
                PlanId = reader.GetInt64(reader.GetOrdinal("PlanId")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                AmountPaid = reader.GetDecimal(reader.GetOrdinal("AmountPaid")),
                PaymentReference = reader.IsDBNull(reader.GetOrdinal("PaymentReference"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("PaymentReference")),
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
                    : reader.GetInt64(reader.GetOrdinal("ModifyBy"))
            };
        }

        public async Task<IEnumerable<TenantSubscription>> GetAllAsync()
        {
            var subscriptions = new List<TenantSubscription>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllTenantSubscription", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                subscriptions.Add(MapSubscription(reader));
            }

            return subscriptions;
        }

        public async Task<TenantSubscription?> GetByIdAsync(long subscriptionId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetByTenantSubscriptionId", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@SubscriptionId", SqlDbType.BigInt).Value = subscriptionId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapSubscription(reader) : null;
        }

        public async Task<long> CreateAsync(TenantSubscription entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("InsertTenantSubscription", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = entity.TenantId;
            command.Parameters.Add("@PlanId", SqlDbType.BigInt).Value = entity.PlanId;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status;

            var amountParameter = command.Parameters.Add("@AmountPaid", SqlDbType.Decimal);
            amountParameter.Precision = 18;
            amountParameter.Scale = 2;
            amountParameter.Value = entity.AmountPaid;

            command.Parameters.Add("@PaymentReference", SqlDbType.NVarChar, 100).Value =
                (object?)entity.PaymentReference ?? DBNull.Value;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)entity.Description ?? DBNull.Value;
            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)entity.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(TenantSubscription entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateTenantSubscription", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@SubscriptionId", SqlDbType.BigInt).Value = entity.SubscriptionId;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = entity.TenantId;
            command.Parameters.Add("@PlanId", SqlDbType.BigInt).Value = entity.PlanId;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status;

            var amountParameter = command.Parameters.Add("@AmountPaid", SqlDbType.Decimal);
            amountParameter.Precision = 18;
            amountParameter.Scale = 2;
            amountParameter.Value = entity.AmountPaid;

            command.Parameters.Add("@PaymentReference", SqlDbType.NVarChar, 100).Value =
                (object?)entity.PaymentReference ?? DBNull.Value;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)entity.Description ?? DBNull.Value;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)entity.ModifyBy ?? DBNull.Value;
            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime).Value = DateTime.Now;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long subscriptionId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("DeleteTenantSubscription", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@SubscriptionId", SqlDbType.BigInt).Value = subscriptionId;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }
    }
}
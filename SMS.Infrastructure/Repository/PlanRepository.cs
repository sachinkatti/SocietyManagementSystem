
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class PlanRepository : IPlanRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public PlanRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Plan>> GetAllAsync()
        {
            var plans = new List<Plan>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.GetAllPlans", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                plans.Add(MapPlan(reader));
            }

            return plans;
        }

        public async Task<Plan?> GetByIdAsync(long planId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.GetPlanById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@PlanId", SqlDbType.BigInt).Value = planId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapPlan(reader) : null;
        }

        public async Task<long> CreateAsync(Plan plan)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.InsertPlan", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PlanName", SqlDbType.NVarChar, 100).Value =
                plan.PlanName;

            command.Parameters.Add("@PlanCode", SqlDbType.NVarChar, 50).Value =
                plan.PlanCode;

            command.Parameters.Add("@BillingCycle", SqlDbType.NVarChar, 20).Value =
                plan.BillingCycle;

            var priceParameter = command.Parameters.Add("@Price", SqlDbType.Decimal);
            priceParameter.Precision = 18;
            priceParameter.Scale = 2;
            priceParameter.Value = plan.Price;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)plan.Description ?? DBNull.Value;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)plan.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Plan plan)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.UpdatePlan", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PlanId", SqlDbType.BigInt).Value =
                plan.PlanId;

            command.Parameters.Add("@PlanName", SqlDbType.NVarChar, 100).Value =
                plan.PlanName;

            command.Parameters.Add("@PlanCode", SqlDbType.NVarChar, 50).Value =
                plan.PlanCode;

            command.Parameters.Add("@BillingCycle", SqlDbType.NVarChar, 20).Value =
                plan.BillingCycle;

            var priceParameter = command.Parameters.Add("@Price", SqlDbType.Decimal);
            priceParameter.Precision = 18;
            priceParameter.Scale = 2;
            priceParameter.Value = plan.Price;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)plan.Description ?? DBNull.Value;

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
                plan.IsActive;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)plan.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long planId, long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.DeletePlan", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PlanId", SqlDbType.BigInt).Value = planId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        private static Plan MapPlan(SqlDataReader reader)
        {
            return new Plan
            {
                PlanId = reader.GetInt64(reader.GetOrdinal("PlanId")),
                PlanName = reader.GetString(reader.GetOrdinal("PlanName")),
                PlanCode = reader.GetString(reader.GetOrdinal("PlanCode")),
                BillingCycle = reader.GetString(reader.GetOrdinal("BillingCycle")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedOn = reader.IsDBNull(reader.GetOrdinal("CreatedOn"))
                    ? null
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
    }
}
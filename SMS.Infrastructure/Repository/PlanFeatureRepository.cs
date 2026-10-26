
using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class PlanFeatureRepository : IPlanFeatureRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public PlanFeatureRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<PlanFeature>> GetAllAsync()
        {
            var items = new List<PlanFeature>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.GetAllPlanFeatures", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                items.Add(MapPlanFeature(reader));
            }

            return items;
        }

        public async Task<PlanFeature?> GetByIdAsync(long planFeatureId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.GetPlanFeatureById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@PlanFeatureId", SqlDbType.BigInt).Value = planFeatureId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapPlanFeature(reader) : null;
        }

        public async Task<long> CreateAsync(PlanFeature planFeature)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.InsertPlanFeature", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PlanId", SqlDbType.BigInt).Value = planFeature.PlanId;
            command.Parameters.Add("@FeatureId", SqlDbType.BigInt).Value = planFeature.FeatureId;
            command.Parameters.Add("@FeatureValue", SqlDbType.NVarChar, 100).Value = planFeature.FeatureValue;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)planFeature.Description ?? DBNull.Value;
            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)planFeature.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(PlanFeature planFeature)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.UpdatePlanFeature", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PlanFeatureId", SqlDbType.BigInt).Value = planFeature.PlanFeatureId;
            command.Parameters.Add("@PlanId", SqlDbType.BigInt).Value = planFeature.PlanId;
            command.Parameters.Add("@FeatureId", SqlDbType.BigInt).Value = planFeature.FeatureId;
            command.Parameters.Add("@FeatureValue", SqlDbType.NVarChar, 100).Value = planFeature.FeatureValue;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)planFeature.Description ?? DBNull.Value;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = planFeature.IsActive;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)planFeature.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
        }

        public async Task<bool> DeleteAsync(long planFeatureId, long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.DeletePlanFeature", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PlanFeatureId", SqlDbType.BigInt).Value = planFeatureId;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
        }

        private static PlanFeature MapPlanFeature(SqlDataReader reader)
        {
            return new PlanFeature
            {
                PlanFeatureId = reader.GetInt64(reader.GetOrdinal("PlanFeatureId")),
                PlanId = reader.GetInt64(reader.GetOrdinal("PlanId")),
                PlanName = reader.GetString(reader.GetOrdinal("PlanName")),
                FeatureId = reader.GetInt64(reader.GetOrdinal("FeatureId")),
                FeatureName = reader.GetString(reader.GetOrdinal("FeatureName")),
                FeatureCode = reader.GetString(reader.GetOrdinal("FeatureCode")),
                FeatureValue = reader.GetString(reader.GetOrdinal("FeatureValue")),
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

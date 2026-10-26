using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public FeatureRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Feature>> GetAllAsync()
        {
            var features = new List<Feature>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.GetAllFeatures", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                features.Add(MapFeature(reader));
            }

            return features;
        }

        public async Task<Feature?> GetByIdAsync(long featureId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.GetFeatureById", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@FeatureId", SqlDbType.BigInt).Value = featureId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapFeature(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Feature feature)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.InsertFeature", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@FeatureName", SqlDbType.NVarChar, 100).Value =
                feature.FeatureName;

            command.Parameters.Add("@FeatureCode", SqlDbType.NVarChar, 50).Value =
                feature.FeatureCode;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)feature.Description ?? DBNull.Value;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)feature.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Feature feature)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.UpdateFeature", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@FeatureId", SqlDbType.BigInt).Value =
                feature.FeatureId;

            command.Parameters.Add("@FeatureName", SqlDbType.NVarChar, 100).Value =
                feature.FeatureName;

            command.Parameters.Add("@FeatureCode", SqlDbType.NVarChar, 50).Value =
                feature.FeatureCode;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)feature.Description ?? DBNull.Value;

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
                feature.IsActive;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)feature.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long featureId, long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("dbo.DeleteFeature", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@FeatureId", SqlDbType.BigInt).Value = featureId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        private static Feature MapFeature(SqlDataReader reader)
        {
            return new Feature
            {
                FeatureId = reader.GetInt64(reader.GetOrdinal("FeatureId")),
                FeatureName = reader.GetString(reader.GetOrdinal("FeatureName")),
                FeatureCode = reader.GetString(reader.GetOrdinal("FeatureCode")),
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
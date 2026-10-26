using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public LanguageRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Language>> GetAllAsync()
        {
            var languages = new List<Language>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetAllLanguage", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                languages.Add(new Language
                {
                    LanguageId = Convert.ToInt64(reader["LanguageId"]),
                    LanguageCode = reader["LanguageCode"]?.ToString() ?? string.Empty,
                    LanguageName = reader["LanguageName"]?.ToString() ?? string.Empty,
                    Description = reader["Description"] == DBNull.Value
                        ? null
                        : reader["Description"].ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                    CreatedBy = reader["CreatedBy"] == DBNull.Value
                        ? null
                        : Convert.ToInt64(reader["CreatedBy"]),
                    ModifyOn = reader["ModifyOn"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["ModifyOn"]),
                    ModifyBy = reader["ModifyBy"] == DBNull.Value
                        ? null
                        : Convert.ToInt64(reader["ModifyBy"])
                });
            }

            return languages;
        }

        public async Task<Language?> GetByIdAsync(long languageId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("GetByLanguageId", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@LanguageId", SqlDbType.BigInt).Value = languageId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Language
            {
                LanguageId = Convert.ToInt64(reader["LanguageId"]),
                LanguageCode = reader["LanguageCode"]?.ToString() ?? string.Empty,
                LanguageName = reader["LanguageName"]?.ToString() ?? string.Empty,
                Description = reader["Description"] == DBNull.Value
                    ? null
                    : reader["Description"].ToString(),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                ModifyOn = reader["ModifyOn"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["ModifyOn"]),
                ModifyBy = reader["ModifyBy"] == DBNull.Value
                    ? null
                    : Convert.ToInt64(reader["ModifyBy"])
            };
        }

        public async Task<long> CreateAsync(Language language)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("InsertLanguage", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@LanguageCode", SqlDbType.NVarChar, 10).Value =
                language.LanguageCode;

            command.Parameters.Add("@LanguageName", SqlDbType.NVarChar, 100).Value =
                language.LanguageName;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)language.Description ?? DBNull.Value;

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
                language.IsActive;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)language.CreatedBy ?? DBNull.Value;

            command.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value =
                language.CreatedOn;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)language.ModifyBy ?? DBNull.Value;

            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime).Value =
                language.ModifyOn ?? (object)DBNull.Value;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            return 0;
        }

        public async Task<bool> UpdateAsync(Language language)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("UpdateLanguage", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@LanguageId", SqlDbType.BigInt).Value =
                language.LanguageId;

            command.Parameters.Add("@LanguageCode", SqlDbType.NVarChar, 10).Value =
                language.LanguageCode;

            command.Parameters.Add("@LanguageName", SqlDbType.NVarChar, 100).Value =
                language.LanguageName;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                (object?)language.Description ?? DBNull.Value;

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
                language.IsActive;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)language.ModifyBy ?? DBNull.Value;

            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime).Value =
                language.ModifyOn ?? (object)DBNull.Value;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(long languageId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("DeleteLanguage", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@LanguageId", SqlDbType.BigInt).Value =
                languageId;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            return true;
        }
    }
}
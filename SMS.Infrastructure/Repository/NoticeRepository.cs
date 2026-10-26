using Microsoft.Data.SqlClient;
using SMS.Domain;

namespace SMS.Infrastructure
{
    public class NoticeRepository : INoticeRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public NoticeRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Notice>> GetAllAsync()
        {
            var notices = new List<Notice>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT NoticeId, Title, Message, Category, PublishDate,
                       PublishedByAdminId, IsActive, CreatedOn, CreatedBy,
                       ModifyOn, ModifyBy, TenantId
                FROM Notices
                WHERE IsActive = 1
                ORDER BY NoticeId DESC";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                notices.Add(MapNotice(reader));
            }

            return notices;
        }

        public async Task<Notice?> GetByIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT NoticeId, Title, Message, Category, PublishDate,
                       PublishedByAdminId, IsActive, CreatedOn, CreatedBy,
                       ModifyOn, ModifyBy, TenantId
                FROM Notices
                WHERE NoticeId = @NoticeId AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NoticeId", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapNotice(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Notice notice)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO Notices
                (
                    Title, Message, Category, PublishDate,
                    PublishedByAdminId, IsActive, CreatedOn,
                    CreatedBy, ModifyOn, ModifyBy, TenantId
                )
                VALUES
                (
                    @Title, @Message, @Category, @PublishDate,
                    @PublishedByAdminId, @IsActive, @CreatedOn,
                    @CreatedBy, @ModifyOn, @ModifyBy, @TenantId
                );

                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using var command = new SqlCommand(query, connection);
            AddParameters(command, notice);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Notice notice)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                UPDATE Notices
                SET Title = @Title,
                    Message = @Message,
                    Category = @Category,
                    PublishDate = @PublishDate,
                    PublishedByAdminId = @PublishedByAdminId,
                    ModifyOn = @ModifyOn,
                    ModifyBy = @ModifyBy,
                    TenantId = @TenantId
                WHERE NoticeId = @NoticeId AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            AddParameters(command, notice);
            command.Parameters.AddWithValue("@NoticeId", notice.NoticeId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                UPDATE Notices
                SET IsActive = 0,
                    ModifyOn = @ModifyOn
                WHERE NoticeId = @NoticeId AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NoticeId", id);
            command.Parameters.AddWithValue("@ModifyOn", DateTime.UtcNow);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static void AddParameters(SqlCommand command, Notice notice)
        {
            command.Parameters.AddWithValue("@Title", notice.Title);
            command.Parameters.AddWithValue("@Message", notice.Message);
            command.Parameters.AddWithValue("@Category", notice.Category);
            command.Parameters.AddWithValue("@PublishDate", notice.PublishDate);
            command.Parameters.AddWithValue("@PublishedByAdminId", notice.PublishedByAdminId);
            command.Parameters.AddWithValue("@IsActive", notice.IsActive);
            command.Parameters.AddWithValue("@CreatedOn", notice.CreatedOn);
            command.Parameters.AddWithValue("@CreatedBy", (object?)notice.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModifyOn", (object?)notice.ModifyOn ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModifyBy", (object?)notice.ModifyBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@TenantId", notice.TenantId);
        }

        private static Notice MapNotice(SqlDataReader reader)
        {
            return new Notice
            {
                NoticeId = reader.GetInt64(reader.GetOrdinal("NoticeId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Message = reader.GetString(reader.GetOrdinal("Message")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                PublishDate = reader.GetDateTime(reader.GetOrdinal("PublishDate")),
                PublishedByAdminId = reader.GetInt64(reader.GetOrdinal("PublishedByAdminId")),
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
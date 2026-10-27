using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class NoticeRepository : INoticeRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public NoticeRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // 1. Get all active notices
        public async Task<List<Notice>> GetAllAsync()
        {
            var notices = new List<Notice>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetAllNotices", connection);

            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                notices.Add(MapNotice(reader));
            }

            return notices;
        }

        // 2. Get notice by ID
        public async Task<Notice?> GetByIdAsync(long noticeId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetNoticeById", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@NoticeId", SqlDbType.BigInt).Value =
                noticeId;

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapNotice(reader);
            }

            return null;
        }

        // 3. Get notices by tenant
        public async Task<List<Notice>> GetByTenantAsync(long tenantId)
        {
            var notices = new List<Notice>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetNoticesByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                tenantId;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                notices.Add(MapNotice(reader));
            }

            return notices;
        }

        // 4. Create notice
        public async Task<long> CreateAsync(Notice notice)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.InsertNotice", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@Title", SqlDbType.NVarChar, 300).Value =
                notice.Title;

            command.Parameters.Add("@Message", SqlDbType.NVarChar, 2000).Value =
                notice.Message;

            command.Parameters.Add("@Category", SqlDbType.NVarChar, 60).Value =
                notice.Category;

            command.Parameters.Add("@PublishDate", SqlDbType.DateTime2).Value =
                notice.PublishDate == default
                    ? DBNull.Value
                    : notice.PublishDate;

            command.Parameters.Add(
                "@PublishedByAdminId", SqlDbType.BigInt).Value =
                notice.PublishedByAdminId;

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                notice.CreatedBy.HasValue
                    ? notice.CreatedBy.Value
                    : DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                notice.TenantId;

            object? result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "Notice could not be created.");
            }

            return Convert.ToInt64(result);
        }

        // 5. Update notice
        public async Task<bool> UpdateAsync(Notice notice)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.UpdateNotice", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@NoticeId", SqlDbType.BigInt).Value =
                notice.NoticeId;

            command.Parameters.Add("@Title", SqlDbType.NVarChar, 300).Value =
                notice.Title;

            command.Parameters.Add("@Message", SqlDbType.NVarChar, 2000).Value =
                notice.Message;

            command.Parameters.Add("@Category", SqlDbType.NVarChar, 60).Value =
                notice.Category;

            command.Parameters.Add("@PublishDate", SqlDbType.DateTime2).Value =
                notice.PublishDate == default
                    ? DBNull.Value
                    : notice.PublishDate;

            command.Parameters.Add(
                "@PublishedByAdminId", SqlDbType.BigInt).Value =
                notice.PublishedByAdminId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                notice.ModifyBy.HasValue
                    ? notice.ModifyBy.Value
                    : DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                notice.TenantId;

            object? result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        // 6. Soft-delete notice
        public async Task<bool> DeleteAsync(
            long noticeId,
            long? modifyBy,
            long tenantId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.DeleteNotice", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@NoticeId", SqlDbType.BigInt).Value =
                noticeId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                modifyBy.HasValue
                    ? modifyBy.Value
                    : DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                tenantId;

            object? result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        // 7. Get active admins for a tenant
        public async Task<List<NoticeAdminOption>> GetAdminsByTenantAsync(
            long tenantId)
        {
            var admins = new List<NoticeAdminOption>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "dbo.GetNoticeAdminsByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value =
                tenantId;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                admins.Add(new NoticeAdminOption
                {
                    AdminId = reader.GetInt64(
                        reader.GetOrdinal("AdminId")),

                    AdminName = reader.IsDBNull(
                        reader.GetOrdinal("AdminName"))
                        ? string.Empty
                        : reader.GetString(
                            reader.GetOrdinal("AdminName"))
                });
            }

            return admins;
        }

        // Common method to map SQL data to Notice entity
        private static Notice MapNotice(SqlDataReader reader)
        {
            return new Notice
            {
                NoticeId = reader.GetInt64(
                    reader.GetOrdinal("NoticeId")),

                Title = reader.GetString(
                    reader.GetOrdinal("Title")),

                Message = reader.GetString(
                    reader.GetOrdinal("Message")),

                Category = reader.GetString(
                    reader.GetOrdinal("Category")),

                PublishDate = reader.GetDateTime(
                    reader.GetOrdinal("PublishDate")),

                PublishedByAdminId = reader.GetInt64(
                    reader.GetOrdinal("PublishedByAdminId")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreatedOn = reader.GetDateTime(
                    reader.GetOrdinal("CreatedOn")),

                CreatedBy = reader.IsDBNull(
                    reader.GetOrdinal("CreatedBy"))
                    ? null
                    : reader.GetInt64(
                        reader.GetOrdinal("CreatedBy")),

                ModifyOn = reader.IsDBNull(
                    reader.GetOrdinal("ModifyOn"))
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("ModifyOn")),

                ModifyBy = reader.IsDBNull(
                    reader.GetOrdinal("ModifyBy"))
                    ? null
                    : reader.GetInt64(
                        reader.GetOrdinal("ModifyBy")),

                TenantId = reader.GetInt64(
                    reader.GetOrdinal("TenantId")),

                TenantName = reader.IsDBNull(
                    reader.GetOrdinal("TenantName"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("TenantName"))
            };
        }
    }
}

using Microsoft.Data.SqlClient;
using SMS.Domain;
using System.Data;

namespace SMS.Infrastructure
{
    public class ComplaintRepository : IComplaintRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ComplaintRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Complaint>> GetAllAsync()
        {
            var complaints = new List<Complaint>();

            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "GetAllComplaints", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                complaints.Add(MapComplaint(reader));
            }

            return complaints;
        }

        public async Task<Complaint?> GetByIdAsync(long complaintId)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "GetComplaintById", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ComplaintId", SqlDbType.BigInt)
                .Value = complaintId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapComplaint(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(Complaint complaint)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "InsertComplaint", connection);

            command.CommandType = CommandType.StoredProcedure;

            AddComplaintParameters(command, complaint);

            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt)
                .Value = (object?)complaint.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Complaint complaint)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "UpdateComplaint", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ComplaintId", SqlDbType.BigInt)
                .Value = complaint.ComplaintId;

            AddComplaintParameters(command, complaint);

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt)
                .Value = (object?)complaint.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(
            long complaintId,
            long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "DeleteComplaint", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ComplaintId", SqlDbType.BigInt)
                .Value = complaintId;

            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt)
                .Value = (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return result != null &&
                   result != DBNull.Value &&
                   Convert.ToInt32(result) > 0;
        }

        public async Task<List<ComplaintResidentOption>>
            GetResidentsByTenantAsync(long tenantId)
        {
            var residents = new List<ComplaintResidentOption>();

            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand(
                "GetComplaintResidentsByTenant", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt)
                .Value = tenantId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                residents.Add(new ComplaintResidentOption
                {
                    ResidentId = reader.GetInt64(
                        reader.GetOrdinal("ResidentId")),

                    ResidentName = reader["ResidentName"]?.ToString()
                                   ?? string.Empty,

                    FlatId = reader.IsDBNull(
                        reader.GetOrdinal("FlatId"))
                        ? null
                        : reader.GetInt64(
                            reader.GetOrdinal("FlatId"))
                });
            }

            return residents;
        }

        private static void AddComplaintParameters(
            SqlCommand command,
            Complaint complaint)
        {
            command.Parameters.Add("@ResidentId", SqlDbType.BigInt)
                .Value = complaint.ResidentId;

            command.Parameters.Add("@Category", SqlDbType.NVarChar, 50)
                .Value = complaint.Category;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500)
                .Value = complaint.Description;

            command.Parameters.Add("@PhotoUrl", SqlDbType.NVarChar, 250)
                .Value = (object?)complaint.PhotoUrl ?? DBNull.Value;

            command.Parameters.Add("@Priority", SqlDbType.NVarChar, 10)
                .Value = complaint.Priority;

            command.Parameters.Add("@Status", SqlDbType.NVarChar, 20)
                .Value = complaint.Status;

            command.Parameters.Add("@DateRaised", SqlDbType.DateTime2)
                .Value = complaint.DateRaised == default
                    ? DBNull.Value
                    : complaint.DateRaised;

            command.Parameters.Add("@DateResolved", SqlDbType.DateTime2)
                .Value = (object?)complaint.DateResolved ?? DBNull.Value;

            command.Parameters.Add(
                "@ResolutionNotes", SqlDbType.NVarChar, 500)
                .Value = (object?)complaint.ResolutionNotes ?? DBNull.Value;

            command.Parameters.Add("@ResidentRating", SqlDbType.Int)
                .Value = (object?)complaint.ResidentRating ?? DBNull.Value;

            command.Parameters.Add("@TenantId", SqlDbType.BigInt)
                .Value = complaint.TenantId;
        }

        private static Complaint MapComplaint(SqlDataReader reader)
        {
            return new Complaint
            {
                ComplaintId = reader.GetInt64(
                    reader.GetOrdinal("ComplaintId")),

                ResidentId = reader.GetInt64(
                    reader.GetOrdinal("ResidentId")),

                ResidentName = GetNullableString(reader, "ResidentName"),

                Category = reader["Category"]?.ToString()
                           ?? string.Empty,

                Description = reader["Description"]?.ToString()
                              ?? string.Empty,

                PhotoUrl = GetNullableString(reader, "PhotoUrl"),

                Priority = reader["Priority"]?.ToString()
                           ?? "Medium",

                Status = reader["Status"]?.ToString()
                         ?? "Open",

                DateRaised = reader.GetDateTime(
                    reader.GetOrdinal("DateRaised")),

                DateResolved = GetNullableDateTime(reader, "DateResolved"),

                ResolutionNotes = GetNullableString(
                    reader, "ResolutionNotes"),

                ResidentRating = reader.IsDBNull(
                    reader.GetOrdinal("ResidentRating"))
                    ? null
                    : reader.GetInt32(
                        reader.GetOrdinal("ResidentRating")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreatedOn = reader.GetDateTime(
                    reader.GetOrdinal("CreatedOn")),

                CreatedBy = GetNullableInt64(reader, "CreatedBy"),

                ModifyOn = GetNullableDateTime(reader, "ModifyOn"),

                ModifyBy = GetNullableInt64(reader, "ModifyBy"),

                TenantId = reader.GetInt64(
                    reader.GetOrdinal("TenantId")),

                TenantName = GetNullableString(reader, "TenantName")
            };
        }

        private static string? GetNullableString(
            SqlDataReader reader, string column)
        {
            int index = reader.GetOrdinal(column);

            return reader.IsDBNull(index)
                ? null
                : reader.GetString(index);
        }

        private static DateTime? GetNullableDateTime(
            SqlDataReader reader, string column)
        {
            int index = reader.GetOrdinal(column);

            return reader.IsDBNull(index)
                ? null
                : reader.GetDateTime(index);
        }

        private static long? GetNullableInt64(
            SqlDataReader reader, string column)
        {
            int index = reader.GetOrdinal(column);

            return reader.IsDBNull(index)
                ? null
                : reader.GetInt64(index);
        }
    }
}

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
                @"SELECT ComplaintId, ResidentId, Category, Description, PhotoUrl,
                         Priority, Status, DateRaised, DateResolved, ResolutionNotes,
                         ResidentRating, IsActive, CreatedOn, CreatedBy, ModifyOn,
                         ModifyBy, TenantId
                  FROM Complaints
                  WHERE IsActive = 1
                  ORDER BY ComplaintId DESC",
                connection);

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
                @"SELECT ComplaintId, ResidentId, Category, Description, PhotoUrl,
                         Priority, Status, DateRaised, DateResolved, ResolutionNotes,
                         ResidentRating, IsActive, CreatedOn, CreatedBy, ModifyOn,
                         ModifyBy, TenantId
                  FROM Complaints
                  WHERE ComplaintId = @ComplaintId AND IsActive = 1",
                connection);

            command.Parameters.Add("@ComplaintId", SqlDbType.BigInt).Value = complaintId;

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
                @"INSERT INTO Complaints
                  (ResidentId, Category, Description, PhotoUrl, Priority, Status,
                   DateRaised, DateResolved, ResolutionNotes, ResidentRating,
                   IsActive, CreatedOn, CreatedBy, TenantId)
                  VALUES
                  (@ResidentId, @Category, @Description, @PhotoUrl, @Priority, @Status,
                   @DateRaised, @DateResolved, @ResolutionNotes, @ResidentRating,
                   @IsActive, @CreatedOn, @CreatedBy, @TenantId);
                  SELECT CAST(SCOPE_IDENTITY() AS BIGINT);",
                connection);

            AddComplaintParameters(command, complaint);

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = true;
            command.Parameters.Add("@CreatedOn", SqlDbType.DateTime2).Value = complaint.CreatedOn;
            command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value =
                (object?)complaint.CreatedBy ?? DBNull.Value;

            await connection.OpenAsync();

            return Convert.ToInt64(await command.ExecuteScalarAsync());
        }

        public async Task<bool> UpdateAsync(Complaint complaint)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"UPDATE Complaints
                  SET ResidentId = @ResidentId,
                      Category = @Category,
                      Description = @Description,
                      PhotoUrl = @PhotoUrl,
                      Priority = @Priority,
                      Status = @Status,
                      DateRaised = @DateRaised,
                      DateResolved = @DateResolved,
                      ResolutionNotes = @ResolutionNotes,
                      ResidentRating = @ResidentRating,
                      TenantId = @TenantId,
                      ModifyOn = @ModifyOn,
                      ModifyBy = @ModifyBy
                  WHERE ComplaintId = @ComplaintId AND IsActive = 1",
                connection);

            AddComplaintParameters(command, complaint);

            command.Parameters.Add("@ComplaintId", SqlDbType.BigInt).Value = complaint.ComplaintId;
            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime2).Value =
                (object?)complaint.ModifyOn ?? DBNull.Value;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)complaint.ModifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long complaintId, long? modifyBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"UPDATE Complaints
                  SET IsActive = 0,
                      ModifyOn = @ModifyOn,
                      ModifyBy = @ModifyBy
                  WHERE ComplaintId = @ComplaintId AND IsActive = 1",
                connection);

            command.Parameters.Add("@ComplaintId", SqlDbType.BigInt).Value = complaintId;
            command.Parameters.Add("@ModifyOn", SqlDbType.DateTime2).Value = DateTime.UtcNow;
            command.Parameters.Add("@ModifyBy", SqlDbType.BigInt).Value =
                (object?)modifyBy ?? DBNull.Value;

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static void AddComplaintParameters(SqlCommand command, Complaint complaint)
        {
            command.Parameters.Add("@ResidentId", SqlDbType.BigInt).Value = complaint.ResidentId;
            command.Parameters.Add("@Category", SqlDbType.NVarChar, 50).Value = complaint.Category;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = complaint.Description;
            command.Parameters.Add("@PhotoUrl", SqlDbType.NVarChar, 250).Value =
                (object?)complaint.PhotoUrl ?? DBNull.Value;
            command.Parameters.Add("@Priority", SqlDbType.NVarChar, 10).Value = complaint.Priority;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = complaint.Status;
            command.Parameters.Add("@DateRaised", SqlDbType.DateTime2).Value = complaint.DateRaised;
            command.Parameters.Add("@DateResolved", SqlDbType.DateTime2).Value =
                (object?)complaint.DateResolved ?? DBNull.Value;
            command.Parameters.Add("@ResolutionNotes", SqlDbType.NVarChar, 500).Value =
                (object?)complaint.ResolutionNotes ?? DBNull.Value;
            command.Parameters.Add("@ResidentRating", SqlDbType.Int).Value =
                (object?)complaint.ResidentRating ?? DBNull.Value;
            command.Parameters.Add("@TenantId", SqlDbType.BigInt).Value = complaint.TenantId;
        }

        private static Complaint MapComplaint(SqlDataReader reader)
        {
            return new Complaint
            {
                ComplaintId = reader.GetInt64(reader.GetOrdinal("ComplaintId")),
                ResidentId = reader.GetInt64(reader.GetOrdinal("ResidentId")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                PhotoUrl = reader["PhotoUrl"] as string,
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                DateRaised = reader.GetDateTime(reader.GetOrdinal("DateRaised")),
                DateResolved = reader["DateResolved"] == DBNull.Value
                    ? null : reader.GetDateTime(reader.GetOrdinal("DateResolved")),
                ResolutionNotes = reader["ResolutionNotes"] as string,
                ResidentRating = reader["ResidentRating"] == DBNull.Value
                    ? null : reader.GetInt32(reader.GetOrdinal("ResidentRating")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                CreatedBy = reader["CreatedBy"] == DBNull.Value
                    ? null : reader.GetInt64(reader.GetOrdinal("CreatedBy")),
                ModifyOn = reader["ModifyOn"] == DBNull.Value
                    ? null : reader.GetDateTime(reader.GetOrdinal("ModifyOn")),
                ModifyBy = reader["ModifyBy"] == DBNull.Value
                    ? null : reader.GetInt64(reader.GetOrdinal("ModifyBy")),
                TenantId = reader.GetInt64(reader.GetOrdinal("TenantId"))
            };
        }
    }
}
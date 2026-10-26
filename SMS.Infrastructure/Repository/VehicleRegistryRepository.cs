using Microsoft.Data.SqlClient;
using SMS.Domain;

namespace SMS.Infrastructure
{
    public class VehicleRegistryRepository : IVehicleRegistryRepository
    {
        private readonly DbConnectionFactory _factory;

        public VehicleRegistryRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<VehicleRegistry>> GetAllAsync()
        {
            var list = new List<VehicleRegistry>();
            using var con = _factory.CreateConnection();
            using var cmd = new SqlCommand(
                "SELECT * FROM VehicleRegistry WHERE IsActive=1 ORDER BY VehicleId DESC", con);

            await con.OpenAsync();
            using var r = await cmd.ExecuteReaderAsync();

            while (await r.ReadAsync())
                list.Add(Map(r));

            return list;
        }

        public async Task<VehicleRegistry?> GetByIdAsync(long id)
        {
            using var con = _factory.CreateConnection();
            using var cmd = new SqlCommand(
                "SELECT * FROM VehicleRegistry WHERE VehicleId=@Id AND IsActive=1", con);
            cmd.Parameters.AddWithValue("@Id", id);

            await con.OpenAsync();
            using var r = await cmd.ExecuteReaderAsync();

            return await r.ReadAsync() ? Map(r) : null;
        }

        public async Task<long> CreateAsync(VehicleRegistry v)
        {
            using var con = _factory.CreateConnection();
            using var cmd = new SqlCommand(@"
                INSERT INTO VehicleRegistry
                (FlatId, VehicleNumber, VehicleType, Description, TenantId,
                 IsActive, CreatedOn, CreatedBy)
                OUTPUT INSERTED.VehicleId
                VALUES (@FlatId, @VehicleNumber, @VehicleType, @Description,
                        @TenantId, 1, SYSDATETIME(), @CreatedBy)", con);

            AddParameters(cmd, v);
            cmd.Parameters.AddWithValue("@CreatedBy", (object?)v.CreatedBy ?? DBNull.Value);

            await con.OpenAsync();
            return Convert.ToInt64(await cmd.ExecuteScalarAsync());
        }

        public async Task<bool> UpdateAsync(VehicleRegistry v)
        {
            using var con = _factory.CreateConnection();
            using var cmd = new SqlCommand(@"
                UPDATE VehicleRegistry SET
                    FlatId=@FlatId, VehicleNumber=@VehicleNumber,
                    VehicleType=@VehicleType, Description=@Description,
                    TenantId=@TenantId, ModifyOn=SYSDATETIME(), ModifyBy=@ModifyBy
                WHERE VehicleId=@Id AND IsActive=1", con);

            AddParameters(cmd, v);
            cmd.Parameters.AddWithValue("@Id", v.VehicleId);
            cmd.Parameters.AddWithValue("@ModifyBy", (object?)v.ModifyBy ?? DBNull.Value);

            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var con = _factory.CreateConnection();
            using var cmd = new SqlCommand(@"
                UPDATE VehicleRegistry SET IsActive=0, ModifyOn=SYSDATETIME()
                WHERE VehicleId=@Id AND IsActive=1", con);
            cmd.Parameters.AddWithValue("@Id", id);

            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static void AddParameters(SqlCommand cmd, VehicleRegistry v)
        {
            cmd.Parameters.AddWithValue("@FlatId", v.FlatId);
            cmd.Parameters.AddWithValue("@VehicleNumber", v.VehicleNumber);
            cmd.Parameters.AddWithValue("@VehicleType", v.VehicleType);
            cmd.Parameters.AddWithValue("@Description", (object?)v.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TenantId", v.TenantId);
        }

        private static VehicleRegistry Map(SqlDataReader r)
        {
            return new VehicleRegistry
            {
                VehicleId = r.GetInt64(r.GetOrdinal("VehicleId")),
                FlatId = r.GetInt64(r.GetOrdinal("FlatId")),
                VehicleNumber = r.GetString(r.GetOrdinal("VehicleNumber")),
                VehicleType = r.GetString(r.GetOrdinal("VehicleType")),
                Description = r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description")),
                TenantId = r.GetInt64(r.GetOrdinal("TenantId")),
                IsActive = r.GetBoolean(r.GetOrdinal("IsActive")),
                CreatedOn = r.GetDateTime(r.GetOrdinal("CreatedOn")),
                CreatedBy = r.IsDBNull(r.GetOrdinal("CreatedBy")) ? null : r.GetInt64(r.GetOrdinal("CreatedBy")),
                ModifyOn = r.IsDBNull(r.GetOrdinal("ModifyOn")) ? null : r.GetDateTime(r.GetOrdinal("ModifyOn")),
                ModifyBy = r.IsDBNull(r.GetOrdinal("ModifyBy")) ? null : r.GetInt64(r.GetOrdinal("ModifyBy"))
            };
        }
    }
}
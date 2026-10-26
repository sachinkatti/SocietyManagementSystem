namespace SMS.Application
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetAllAsync();
        Task<PermissionDto?> GetByIdAsync(long permissionId);
        Task<long> CreateAsync(PermissionDto dto);
        Task<bool> UpdateAsync(PermissionDto dto);
        Task<bool> DeleteAsync(long permissionId, long? modifyBy);
    }
}

using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<List<AdminDto>> GetAllAsync()
        {
            var admins = await _adminRepository.GetAllAsync();

            return admins.Select(MapToDto).ToList();
        }

        public async Task<AdminDto?> GetByIdAsync(long adminId)
        {
            var admin = await _adminRepository.GetByIdAsync(adminId);

            return admin == null ? null : MapToDto(admin);
        }

        public async Task<long> CreateAsync(AdminDto admin, long? createdBy)
        {
            var entity = MapToEntity(admin);

            return await _adminRepository.CreateAsync(entity, createdBy);
        }

        public async Task<bool> UpdateAsync(AdminDto admin, long? modifyBy)
        {
            var entity = MapToEntity(admin);

            return await _adminRepository.UpdateAsync(entity, modifyBy);
        }

        public async Task<bool> DeleteAsync(long adminId, long? modifyBy)
        {
            return await _adminRepository.DeleteAsync(adminId, modifyBy);
        }

        private static AdminDto MapToDto(Admin admin)
        {
            return new AdminDto
            {
                AdminId = admin.AdminId,
                UserId = admin.UserId,
                TenantId = admin.TenantId,
                FirstName = admin.FirstName,
                MiddleName = admin.MiddleName,
                LastName = admin.LastName,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                Designation = admin.Designation,
                ContactNumber = admin.ContactNumber,
                DateAssigned = admin.DateAssigned,
                IsActive = admin.IsActive,
                CreatedOn = admin.CreatedOn,
                CreatedBy = admin.CreatedBy,
                ModifyOn = admin.ModifyOn,
                ModifyBy = admin.ModifyBy
            };
        }


        public async Task<List<UserDto>> GetAvailableUsersForAdminAsync()
        {
            var users = await _adminRepository.GetAvailableUsersForAdminAsync();

            return users.Select(user => new UserDto
            {
                UserId = user.UserId,
                TenantId = user.TenantId,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            }).ToList();
        }

        private static Admin MapToEntity(AdminDto admin)
        {
            return new Admin
            {
                AdminId = admin.AdminId,
                UserId = admin.UserId,
                TenantId = admin.TenantId,
                FirstName = admin.FirstName,
                MiddleName = admin.MiddleName,
                LastName = admin.LastName,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                Designation = admin.Designation,
                ContactNumber = admin.ContactNumber,
                DateAssigned = admin.DateAssigned,
                IsActive = admin.IsActive,
                CreatedOn = admin.CreatedOn,
                CreatedBy = admin.CreatedBy,
                ModifyOn = admin.ModifyOn,
                ModifyBy = admin.ModifyBy
            };
        }
    }
}

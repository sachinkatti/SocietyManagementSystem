using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();
            return users.Select(ToDto);
        }

        public async Task<UserDto?> GetByIdAsync(long userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            return user == null ? null : ToDto(user);
        }

        public async Task<long> CreateAsync(UserDto dto)
        {
            return await _repository.CreateAsync(ToEntity(dto));
        }

        public async Task<bool> UpdateAsync(UserDto dto)
        {
            return await _repository.UpdateAsync(ToEntity(dto));
        }

        public Task<bool> DeleteAsync(long userId)
        {
            return _repository.DeleteAsync(userId);
        }

        public async Task<IEnumerable<UserDto>> GetActiveUsersForDropdownAsync()
        {
            var users = await _repository.GetActiveUsersForDropdownAsync();

            return users.Select(user => new UserDto
            {
                UserId = user.UserId,
                TenantId = user.TenantId,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserDescription = user.UserDescription,
                IsActive = user.IsActive
            });
        }

        private static UserDto ToDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                TenantId = user.TenantId,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserDescription = user.UserDescription,
                IsActive = user.IsActive,
                CreatedOn = user.CreatedOn,
                CreatedBy = user.CreatedBy,
                ModifyOn = user.ModifyOn,
                ModifyBy = user.ModifyBy
            };
        }

        private static User ToEntity(UserDto dto)
        {
            return new User
            {
                UserId = dto.UserId,
                TenantId = dto.TenantId,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                UserDescription = dto.UserDescription,
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy,
                ModifyBy = dto.ModifyBy
            };
        }
    }
}

using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class ResidentService : IResidentService
    {
        private readonly IResidentRepository _residentRepository;

        public ResidentService(IResidentRepository residentRepository)
        {
            _residentRepository = residentRepository;
        }

        public async Task<IEnumerable<ResidentDto>> GetAllAsync()
        {
            var residents = await _residentRepository.GetAllAsync();

            return residents.Select(MapToDto).ToList();
        }

        public async Task<ResidentDto?> GetByIdAsync(long residentId)
        {
            var resident =
                await _residentRepository.GetByIdAsync(residentId);

            return resident == null ? null : MapToDto(resident);
        }

        public async Task<long> CreateAsync(ResidentDto dto)
        {
            var resident = MapToEntity(dto);

            return await _residentRepository.CreateAsync(resident);
        }

        public async Task<bool> UpdateAsync(ResidentDto dto)
        {
            var resident = MapToEntity(dto);

            return await _residentRepository.UpdateAsync(resident);
        }

        public async Task<bool> DeleteAsync(long residentId)
        {
            return await _residentRepository.DeleteAsync(residentId);
        }

        private static ResidentDto MapToDto(Resident resident)
        {
            return new ResidentDto
            {
                ResidentId = resident.ResidentId,

                UserId = resident.UserId,
                FullName = resident.FullName,

                FlatId = resident.FlatId,
                FlatNumber = resident.FlatNumber,
                WingName = resident.WingName,

                ResidentType = resident.ResidentType,
                MoveInDate = resident.MoveInDate,
                EmergencyContact = resident.EmergencyContact,

                IsActive = resident.IsActive,
                CreatedOn = resident.CreatedOn,
                CreatedBy = resident.CreatedBy,
                ModifyOn = resident.ModifyOn,
                ModifyBy = resident.ModifyBy
            };
        }

        private static Resident MapToEntity(ResidentDto dto)
        {
            return new Resident
            {
                ResidentId = dto.ResidentId,

                UserId = dto.UserId,
                FlatId = dto.FlatId,

                ResidentType = dto.ResidentType,
                MoveInDate = dto.MoveInDate,
                EmergencyContact = dto.EmergencyContact,

                CreatedBy = dto.CreatedBy,
                ModifyBy = dto.ModifyBy
            };
        }
    }
}

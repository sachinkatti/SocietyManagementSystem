using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class TalukService : ITalukService
    {
        private readonly ITalukRepository _talukRepository;

        public TalukService(ITalukRepository talukRepository)
        {
            _talukRepository = talukRepository;
        }

        public async Task<IEnumerable<TalukDto>> GetAllAsync()
        {
            var taluks = await _talukRepository.GetAllAsync();

            return taluks.Select(MapToDto);
        }

        public async Task<TalukDto?> GetByIdAsync(long talukId)
        {
            var taluk =
                await _talukRepository.GetByIdAsync(talukId);

            return taluk == null
                ? null
                : MapToDto(taluk);
        }

        public async Task<long> CreateAsync(TalukDto dto)
        {
            var taluk = new Taluk
            {
                DistrictId = dto.DistrictId,
                TalukName = dto.TalukName.Trim(),
                TalukCode = dto.TalukCode.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = true,
                CreatedOn = DateTime.Now,
                CreatedBy = 1
            };

            return await _talukRepository.CreateAsync(taluk);
        }

        public async Task<bool> UpdateAsync(TalukDto dto)
        {
            var taluk = new Taluk
            {
                TalukId = dto.TalukId,
                DistrictId = dto.DistrictId,
                TalukName = dto.TalukName.Trim(),
                TalukCode = dto.TalukCode.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = dto.IsActive,
                ModifyOn = DateTime.Now,
                ModifyBy = 1
            };

            return await _talukRepository.UpdateAsync(taluk);
        }

        public async Task<bool> DeleteAsync(long talukId)
        {
            return await _talukRepository.DeleteAsync(talukId);
        }

        private static TalukDto MapToDto(Taluk taluk)
        {
            return new TalukDto
            {
                TalukId = taluk.TalukId,
                DistrictId = taluk.DistrictId,
                TalukName = taluk.TalukName,
                TalukCode = taluk.TalukCode,
                Description = taluk.Description,
                IsActive = taluk.IsActive,
                CreatedOn = taluk.CreatedOn,
                CreatedBy = taluk.CreatedBy,
                ModifyOn = taluk.ModifyOn,
                ModifyBy = taluk.ModifyBy
            };
        }
    }
}
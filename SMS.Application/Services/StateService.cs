using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;

        public StateService(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public async Task<IEnumerable<StateDto>> GetAllAsync()
        {
            var states = await _stateRepository.GetAllAsync();

            return states.Select(MapToDto);
        }

        public async Task<StateDto?> GetByIdAsync(long stateId)
        {
            var state = await _stateRepository.GetByIdAsync(stateId);

            return state == null ? null : MapToDto(state);
        }

        public async Task<long> CreateAsync(StateDto dto)
        {
            var state = new State
            {
                CountryId = dto.CountryId,
                StateName = dto.StateName.Trim(),
                StateCode = dto.StateCode.Trim(),
                IsActive = true,
                CreatedOn = DateTime.Now,
                CreatedBy = 1
            };

            return await _stateRepository.CreateAsync(state);
        }

        public async Task<bool> UpdateAsync(StateDto dto)
        {
            var state = new State
            {
                StateId = dto.StateId,
                CountryId = dto.CountryId,
                StateName = dto.StateName.Trim(),
                StateCode = dto.StateCode.Trim(),
                IsActive = dto.IsActive,
                ModifyOn = DateTime.Now,
                ModifyBy = 1
            };

            return await _stateRepository.UpdateAsync(state);
        }

        public async Task<bool> DeleteAsync(long stateId)
        {
            return await _stateRepository.DeleteAsync(stateId);
        }

        private static StateDto MapToDto(State state)
        {
            return new StateDto
            {
                StateId = state.StateId,
                CountryId = state.CountryId,
                StateName = state.StateName,
                StateCode = state.StateCode,
                IsActive = state.IsActive,
                CreatedOn = state.CreatedOn,
                CreatedBy = state.CreatedBy,
                ModifyOn = state.ModifyOn,
                ModifyBy = state.ModifyBy
            };
        }
    }
}
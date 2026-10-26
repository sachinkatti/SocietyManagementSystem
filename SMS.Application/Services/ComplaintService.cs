using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository _repository;

        public ComplaintService(IComplaintRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ComplaintDto>> GetAllAsync()
        {
            var complaints = await _repository.GetAllAsync();
            return complaints.Select(MapToDto).ToList();
        }

        public async Task<ComplaintDto?> GetByIdAsync(long complaintId)
        {
            var complaint = await _repository.GetByIdAsync(complaintId);
            return complaint == null ? null : MapToDto(complaint);
        }

        public async Task<long> CreateAsync(ComplaintDto dto)
        {
            var complaint = MapToEntity(dto);
            complaint.IsActive = true;
            complaint.CreatedOn = DateTime.UtcNow;

            return await _repository.CreateAsync(complaint);
        }

        public async Task<bool> UpdateAsync(ComplaintDto dto)
        {
            var complaint = MapToEntity(dto);
            complaint.ModifyOn = DateTime.UtcNow;

            return await _repository.UpdateAsync(complaint);
        }

        public Task<bool> DeleteAsync(long complaintId, long? modifyBy)
        {
            return _repository.DeleteAsync(complaintId, modifyBy);
        }

        private static ComplaintDto MapToDto(Complaint complaint)
        {
            return new ComplaintDto
            {
                ComplaintId = complaint.ComplaintId,
                ResidentId = complaint.ResidentId,
                Category = complaint.Category,
                Description = complaint.Description,
                PhotoUrl = complaint.PhotoUrl,
                Priority = complaint.Priority,
                Status = complaint.Status,
                DateRaised = complaint.DateRaised,
                DateResolved = complaint.DateResolved,
                ResolutionNotes = complaint.ResolutionNotes,
                ResidentRating = complaint.ResidentRating,
                IsActive = complaint.IsActive,
                CreatedOn = complaint.CreatedOn,
                CreatedBy = complaint.CreatedBy,
                ModifyOn = complaint.ModifyOn,
                ModifyBy = complaint.ModifyBy,
                TenantId = complaint.TenantId
            };
        }

        private static Complaint MapToEntity(ComplaintDto dto)
        {
            return new Complaint
            {
                ComplaintId = dto.ComplaintId,
                ResidentId = dto.ResidentId,
                Category = dto.Category,
                Description = dto.Description,
                PhotoUrl = dto.PhotoUrl,
                Priority = dto.Priority,
                Status = dto.Status,
                DateRaised = dto.DateRaised,
                DateResolved = dto.DateResolved,
                ResolutionNotes = dto.ResolutionNotes,
                ResidentRating = dto.ResidentRating,
                IsActive = dto.IsActive,
                CreatedOn = dto.CreatedOn,
                CreatedBy = dto.CreatedBy,
                ModifyOn = dto.ModifyOn,
                ModifyBy = dto.ModifyBy,
                TenantId = dto.TenantId
            };
        }
    }
}
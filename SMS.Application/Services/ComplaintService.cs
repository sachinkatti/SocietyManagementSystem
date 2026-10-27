using SMS.Domain;
using SMS.Infrastructure;

namespace SMS.Application
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository _complaintRepository;

        public ComplaintService(IComplaintRepository complaintRepository)
        {
            _complaintRepository = complaintRepository;
        }

        public async Task<List<ComplaintDto>> GetAllAsync()
        {
            var complaints = await _complaintRepository.GetAllAsync();

            return complaints.Select(MapToDto).ToList();
        }

        public async Task<ComplaintDto?> GetByIdAsync(long complaintId)
        {
            if (complaintId <= 0)
                throw new ArgumentException("Invalid complaint ID.");

            var complaint =
                await _complaintRepository.GetByIdAsync(complaintId);

            return complaint == null ? null : MapToDto(complaint);
        }

        public async Task<long> CreateAsync(ComplaintDto complaintDto)
        {
            ValidateComplaint(complaintDto);

            var complaint = MapToEntity(complaintDto);

            complaint.DateRaised = complaintDto.DateRaised == default
                ? DateTime.Now
                : complaintDto.DateRaised;

            complaint.Status = string.IsNullOrWhiteSpace(complaintDto.Status)
                ? "Open"
                : complaintDto.Status.Trim();

            complaint.Priority = complaintDto.Priority.Trim();
            complaint.Category = complaintDto.Category.Trim();
            complaint.Description = complaintDto.Description.Trim();

            return await _complaintRepository.CreateAsync(complaint);
        }

        public async Task<bool> UpdateAsync(ComplaintDto complaintDto)
        {
            if (complaintDto.ComplaintId <= 0)
                throw new ArgumentException("Invalid complaint ID.");

            ValidateComplaint(complaintDto);

            var complaint = MapToEntity(complaintDto);

            complaint.Category = complaintDto.Category.Trim();
            complaint.Description = complaintDto.Description.Trim();
            complaint.Priority = complaintDto.Priority.Trim();
            complaint.Status = complaintDto.Status.Trim();
            complaint.ModifyOn = DateTime.Now;

            return await _complaintRepository.UpdateAsync(complaint);
        }

        public async Task<bool> DeleteAsync(
            long complaintId, long? modifyBy)
        {
            if (complaintId <= 0)
                throw new ArgumentException("Invalid complaint ID.");

            return await _complaintRepository.DeleteAsync(
                complaintId, modifyBy);
        }

        public async Task<List<ComplaintResidentOption>>
            GetResidentsByTenantAsync(long tenantId)
        {
            if (tenantId <= 0)
                throw new ArgumentException("Invalid tenant ID.");

            return await _complaintRepository
                .GetResidentsByTenantAsync(tenantId);
        }

        private static void ValidateComplaint(ComplaintDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.ResidentId <= 0)
                throw new ArgumentException("Please select a resident.");

            if (dto.TenantId <= 0)
                throw new ArgumentException("Invalid tenant ID.");

            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new ArgumentException("Category is required.");

            if (dto.Category.Trim().Length > 50)
                throw new ArgumentException(
                    "Category cannot exceed 50 characters.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ArgumentException("Description is required.");

            if (dto.Description.Trim().Length > 500)
                throw new ArgumentException(
                    "Description cannot exceed 500 characters.");

            if (dto.PhotoUrl?.Length > 250)
                throw new ArgumentException(
                    "Photo URL cannot exceed 250 characters.");

            string[] validPriorities = { "Low", "Medium", "High" };

            if (string.IsNullOrWhiteSpace(dto.Priority) ||
                !validPriorities.Contains(dto.Priority.Trim(),
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Priority must be Low, Medium, or High.");
            }

            string[] validStatuses =
            {
                "Open", "In Progress", "Resolved", "Closed", "Rejected"
            };

            if (string.IsNullOrWhiteSpace(dto.Status) ||
                !validStatuses.Contains(dto.Status.Trim(),
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Please select a valid complaint status.");
            }

            if (dto.ResolutionNotes?.Length > 500)
                throw new ArgumentException(
                    "Resolution notes cannot exceed 500 characters.");

            if (dto.ResidentRating.HasValue &&
                (dto.ResidentRating < 1 || dto.ResidentRating > 5))
            {
                throw new ArgumentException(
                    "Resident rating must be between 1 and 5.");
            }

            if (dto.DateResolved.HasValue &&
                dto.DateRaised != default &&
                dto.DateResolved.Value < dto.DateRaised)
            {
                throw new ArgumentException(
                    "Resolution date cannot be earlier than the raised date.");
            }
        }

        private static ComplaintDto MapToDto(Complaint complaint)
        {
            return new ComplaintDto
            {
                ComplaintId = complaint.ComplaintId,
                ResidentId = complaint.ResidentId,
                ResidentName = complaint.ResidentName,
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
                TenantId = complaint.TenantId,
                TenantName = complaint.TenantName
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
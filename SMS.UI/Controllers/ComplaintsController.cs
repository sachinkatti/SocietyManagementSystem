using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly IComplaintService _complaintService;
        private readonly IResidentService _residentService;
        private readonly IUserService _userService;
        private readonly IFlatService _flatService;

        public ComplaintsController(
            IComplaintService complaintService,
            IResidentService residentService,
            IUserService userService,
            IFlatService flatService)
        {
            _complaintService = complaintService;
            _residentService = residentService;
            _userService = userService;
            _flatService = flatService;
        }

        public async Task<IActionResult> Index()
        {
            var complaints = await _complaintService.GetAllAsync();
            await PopulateResidentNames(complaints);

            return View(complaints);
        }

        public async Task<IActionResult> Details(long id)
        {
            var dto = await _complaintService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            await PopulateResidentNames(new List<ComplaintDto> { dto });

            return View(dto);
        }

        public async Task<IActionResult> Create()
        {
            var dto = new ComplaintDto
            {
                DateRaised = DateTime.Now,
                Status = "Open",
                Priority = "Medium"
            };

            await PopulateDropdowns();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplaintDto dto)
        {
            ValidateComplaint(dto);

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(dto.ResidentId);
                return View(dto);
            }

            dto.IsActive = true;

            await _complaintService.CreateAsync(dto);

            TempData["SuccessMessage"] = "Complaint created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var dto = await _complaintService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            await PopulateDropdowns(dto.ResidentId);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ComplaintDto dto)
        {
            if (id != dto.ComplaintId)
                return BadRequest();

            ValidateComplaint(dto);

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(dto.ResidentId);
                return View(dto);
            }

            var updated = await _complaintService.UpdateAsync(dto);

            if (!updated)
                return NotFound();

            TempData["SuccessMessage"] = "Complaint updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var dto = await _complaintService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            await PopulateResidentNames(new List<ComplaintDto> { dto });

            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _complaintService.DeleteAsync(id, null);

            if (!deleted)
                return NotFound();

            TempData["SuccessMessage"] = "Complaint deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private void ValidateComplaint(ComplaintDto dto)
        {
            if (dto.ResidentId <= 0)
                ModelState.AddModelError(nameof(dto.ResidentId), "Please select a resident.");

            if (string.IsNullOrWhiteSpace(dto.Category))
                ModelState.AddModelError(nameof(dto.Category), "Category is required.");
            else if (dto.Category.Length > 50)
                ModelState.AddModelError(nameof(dto.Category), "Category cannot exceed 50 characters.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                ModelState.AddModelError(nameof(dto.Description), "Description is required.");
            else if (dto.Description.Length > 500)
                ModelState.AddModelError(nameof(dto.Description), "Description cannot exceed 500 characters.");

            if (dto.PhotoUrl?.Length > 250)
                ModelState.AddModelError(nameof(dto.PhotoUrl), "Photo URL cannot exceed 250 characters.");

            if (string.IsNullOrWhiteSpace(dto.Priority))
                ModelState.AddModelError(nameof(dto.Priority), "Priority is required.");
            else if (dto.Priority.Length > 10)
                ModelState.AddModelError(nameof(dto.Priority), "Priority cannot exceed 10 characters.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                ModelState.AddModelError(nameof(dto.Status), "Status is required.");
            else if (dto.Status.Length > 20)
                ModelState.AddModelError(nameof(dto.Status), "Status cannot exceed 20 characters.");

            if (dto.DateRaised == default)
                ModelState.AddModelError(nameof(dto.DateRaised), "Date raised is required.");

            if (dto.DateResolved.HasValue && dto.DateResolved.Value < dto.DateRaised)
                ModelState.AddModelError(nameof(dto.DateResolved), "Resolution date cannot be earlier than the raised date.");

            if (dto.ResolutionNotes?.Length > 500)
                ModelState.AddModelError(nameof(dto.ResolutionNotes), "Resolution notes cannot exceed 500 characters.");

            if (dto.ResidentRating.HasValue &&
                (dto.ResidentRating.Value < 1 || dto.ResidentRating.Value > 5))
            {
                ModelState.AddModelError(nameof(dto.ResidentRating), "Resident rating must be between 1 and 5.");
            }

            if (dto.TenantId <= 0)
                ModelState.AddModelError(nameof(dto.TenantId), "Tenant ID must be greater than zero.");
        }

        private async Task PopulateDropdowns(long? selectedResidentId = null)
        {
            var residents = await _residentService.GetAllAsync();
            var users = await _userService.GetActiveUsersForDropdownAsync();
            var flats = await _flatService.GetAllAsync();

            var userLookup = users.ToDictionary(u => u.UserId);
            var flatLookup = flats.ToDictionary(f => f.FlatId, f => f.FlatNumber);

            var residentItems = residents.Select(r =>
            {
                var name = userLookup.TryGetValue(r.UserId, out var user)
                    ? $"{user.FirstName} {user.LastName}"
                    : $"Resident {r.ResidentId}";

                var flatNumber = flatLookup.TryGetValue(r.FlatId, out var flat)
                    ? flat
                    : "Unknown Flat";

                return new SelectListItem
                {
                    Value = r.ResidentId.ToString(),
                    Text = $"{name} - Flat {flatNumber}",
                    Selected = selectedResidentId == r.ResidentId
                };
            }).ToList();

            ViewBag.Residents = residentItems;
        }

        private async Task PopulateResidentNames(List<ComplaintDto> complaints)
        {
            var residents = await _residentService.GetAllAsync();
            var users = await _userService.GetActiveUsersForDropdownAsync();
            var flats = await _flatService.GetAllAsync();

            var residentLookup = residents.ToDictionary(r => r.ResidentId);
            var userLookup = users.ToDictionary(u => u.UserId);
            var flatLookup = flats.ToDictionary(f => f.FlatId, f => f.FlatNumber);

            foreach (var complaint in complaints)
            {
                if (!residentLookup.TryGetValue(complaint.ResidentId, out var resident))
                {
                    complaint.ResidentName = "Unknown Resident";
                    complaint.FlatNumber = "Unknown";
                    continue;
                }

                complaint.ResidentName = userLookup.TryGetValue(resident.UserId, out var user)
                    ? $"{user.FirstName} {user.LastName}"
                    : $"Resident {resident.ResidentId}";

                complaint.FlatNumber = flatLookup.TryGetValue(resident.FlatId, out var flatNumber)
                    ? flatNumber
                    : "Unknown";
            }
        }
    }
}
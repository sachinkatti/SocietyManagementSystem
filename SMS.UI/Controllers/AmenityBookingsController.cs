
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class AmenityBookingsController : Controller
    {
        private readonly IAmenityBookingService _bookingService;
        private readonly IAmenityService _amenityService;
        private readonly IFlatService _flatService;

        public AmenityBookingsController(
            IAmenityBookingService bookingService,
            IAmenityService amenityService,
            IFlatService flatService)
        {
            _bookingService = bookingService;
            _amenityService = amenityService;
            _flatService = flatService;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = (await _bookingService.GetAllAsync()).ToList();
            var amenities = (await _amenityService.GetAllAsync()).ToDictionary(x => x.AmenityId);
            var flats = (await _flatService.GetAllAsync()).ToDictionary(x => x.FlatId);

            foreach (var booking in bookings)
            {
                if (amenities.TryGetValue(booking.AmenityId, out var amenity))
                    booking.AmenityName = amenity.Name;

                if (flats.TryGetValue(booking.FlatId, out var flat))
                    booking.FlatNumber = flat.FlatNumber;
            }

            return View(bookings);
        }

        public async Task<IActionResult> Details(long id)
        {
            var booking = await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound();

            await LoadDropdownsAsync(booking.AmenityId, booking.FlatId);
            await SetDisplayNamesAsync(booking);

            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();

            return View(new AmenityBookingDto
            {
                SlotDate = DateTime.Today,
                Status = "Pending"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AmenityBookingDto dto)
        {
            ValidateBooking(dto);

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(dto.AmenityId, dto.FlatId);
                return View(dto);
            }

            await _bookingService.CreateAsync(dto);

            TempData["Success"] = "Amenity booking created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var booking = await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound();

            await LoadDropdownsAsync(booking.AmenityId, booking.FlatId);

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, AmenityBookingDto dto)
        {
            if (id != dto.BookingId)
                return BadRequest();

            ValidateBooking(dto);

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(dto.AmenityId, dto.FlatId);
                return View(dto);
            }

            var updated = await _bookingService.UpdateAsync(dto);

            if (!updated)
                return NotFound();

            TempData["Success"] = "Amenity booking updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var booking = await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound();

            await SetDisplayNamesAsync(booking);

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _bookingService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["Success"] = "Amenity booking deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync(long? selectedAmenityId = null, long? selectedFlatId = null)
        {
            var amenities = await _amenityService.GetAllAsync();
            var flats = await _flatService.GetAllAsync();

            ViewBag.Amenities = new SelectList(
                amenities,
                "AmenityId",
                "Name",
                selectedAmenityId);

            ViewBag.Flats = new SelectList(
                flats,
                "FlatId",
                "FlatNumber",
                selectedFlatId);

            ViewBag.Statuses = new SelectList(
                new[] { "Pending", "Confirmed", "Completed", "Cancelled" });
        }

        private async Task SetDisplayNamesAsync(AmenityBookingDto booking)
        {
            var amenity = await _amenityService.GetByIdAsync(booking.AmenityId);
            var flat = (await _flatService.GetAllAsync())
                .FirstOrDefault(x => x.FlatId == booking.FlatId);

            booking.AmenityName = amenity?.Name;
            booking.FlatNumber = flat?.FlatNumber;
        }

        private void ValidateBooking(AmenityBookingDto dto)
        {
            if (dto.AmenityId <= 0)
                ModelState.AddModelError(nameof(dto.AmenityId), "Select an amenity.");

            if (dto.FlatId <= 0)
                ModelState.AddModelError(nameof(dto.FlatId), "Select a flat.");

            if (dto.SlotDate == default)
                ModelState.AddModelError(nameof(dto.SlotDate), "Select a booking date.");

            if (string.IsNullOrWhiteSpace(dto.SlotTime) || dto.SlotTime.Trim().Length > 20)
                ModelState.AddModelError(nameof(dto.SlotTime), "Enter a slot time of up to 20 characters.");

            if (dto.DepositPaid < 0)
                ModelState.AddModelError(nameof(dto.DepositPaid), "Deposit cannot be negative.");

            if (string.IsNullOrWhiteSpace(dto.Status) || dto.Status.Trim().Length > 20)
                ModelState.AddModelError(nameof(dto.Status), "Select a valid status.");

            if (dto.TenantId <= 0)
                ModelState.AddModelError(nameof(dto.TenantId), "Enter a valid Tenant ID.");
        }
    }
}

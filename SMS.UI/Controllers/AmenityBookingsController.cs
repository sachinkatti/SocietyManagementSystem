
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Application;

namespace SMS.UI.Controllers
{
    public class AmenityBookingsController : Controller
    {
        private readonly IAmenityBookingService _bookingService;
        private readonly ITenantService _tenantService;

        public AmenityBookingsController(
            IAmenityBookingService bookingService,
            ITenantService tenantService)
        {
            _bookingService = bookingService;
            _tenantService = tenantService;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingService.GetAllAsync();
            return View(bookings);
        }

        public async Task<IActionResult> Details(long id)
        {
            var booking = await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        public async Task<IActionResult> Create()
        {
            await LoadTenantsAsync();

            return View(new AmenityBookingDto
            {
                SlotDate = DateTime.Today,
                Status = "Booked"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AmenityBookingDto booking)
        {
            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync();
                await LoadBookingOptionsAsync(booking.TenantId, booking);
                return View(booking);
            }

            try
            {
                await _bookingService.CreateAsync(booking);

                TempData["SuccessMessage"] =
                    "Amenity booking created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await LoadTenantsAsync();
                await LoadBookingOptionsAsync(booking.TenantId, booking);

                return View(booking);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var booking = await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound();

            await LoadTenantsAsync();
            await LoadBookingOptionsAsync(booking.TenantId, booking);

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            long id, AmenityBookingDto booking)
        {
            if (id != booking.BookingId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadTenantsAsync();
                await LoadBookingOptionsAsync(booking.TenantId, booking);
                return View(booking);
            }

            try
            {
                var updated = await _bookingService.UpdateAsync(booking);

                if (!updated)
                    return NotFound();

                TempData["SuccessMessage"] =
                    "Amenity booking updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await LoadTenantsAsync();
                await LoadBookingOptionsAsync(booking.TenantId, booking);

                return View(booking);
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var booking = await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _bookingService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["SuccessMessage"] =
                "Amenity booking deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetAmenitiesByTenant(long tenantId)
        {
            if (tenantId <= 0)
                return Json(Array.Empty<object>());

            var amenities =
                await _bookingService.GetAmenitiesByTenantAsync(tenantId);

            return Json(amenities.Select(a => new
            {
                id = a.AmenityId,
                text = a.AmenityName
            }));
        }

        [HttpGet]
        public async Task<IActionResult> GetFlatsByTenant(long tenantId)
        {
            if (tenantId <= 0)
                return Json(Array.Empty<object>());

            var flats =
                await _bookingService.GetFlatsByTenantAsync(tenantId);

            return Json(flats.Select(f => new
            {
                id = f.FlatId,
                text = $"{f.FlatNumber} - {f.WingName}"
            }));
        }

        private async Task LoadTenantsAsync()
        {
            var tenants = await _tenantService.GetAllAsync();

            ViewBag.Tenants = new SelectList(
                tenants.Where(t => t.IsActive),
                "TenantId",
                "TenantName");
        }

        private async Task LoadBookingOptionsAsync(
            long tenantId, AmenityBookingDto booking)
        {
            var amenities = tenantId > 0
                ? await _bookingService.GetAmenitiesByTenantAsync(tenantId)
                : Enumerable.Empty<
                    SMS.Infrastructure.AmenityBookingAmenityOption>();

            var flats = tenantId > 0
                ? await _bookingService.GetFlatsByTenantAsync(tenantId)
                : Enumerable.Empty<
                    SMS.Infrastructure.AmenityBookingFlatOption>();

            ViewBag.Amenities = new SelectList(
                amenities, "AmenityId", "AmenityName", booking.AmenityId);

            ViewBag.Flats = new SelectList(
                flats.Select(f => new
                {
                    f.FlatId,
                    FlatDisplay = $"{f.FlatNumber} - {f.WingName}"
                }),
                "FlatId",
                "FlatDisplay",
                booking.FlatId);
        }
    }
}
